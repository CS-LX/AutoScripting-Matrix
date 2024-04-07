using Engine;
using Engine.Graphics;
using GameEntitySystem;

namespace Game {
    public class ASMPortal : IDisposable {
        public ASMComplexPerspectiveCamera m_camera;
        public GameWidget m_gameWidget;
        public Matrix m_transformMatrix;
        public Matrix TransformMatrix => m_transformMatrix;
        public Matrix TransformMatrixWithoutScale => ASMStaticMethods.RemoveScale(m_transformMatrix);
        public Vector3 m_lastViewTranslation;
        public ComponentASMPortal m_controller;
        public ComponentPlayer m_player;
        public int m_screenUVSubdivision;
        public bool m_teleportEnable = true;
        public float m_offsetY = 0;

        public readonly Project Project;

        readonly SubsystemTerrain m_subsystemTerrain;
        readonly SubsystemASMCamerasGameWidgets m_subsystemCameraGameWidgets;
        readonly SubsystemGameWidgets m_subsystemGameWidgets;
        readonly SubsystemSky m_subsystemSky;
        readonly SubsystemDrawing m_subsystemDrawing;
        readonly SubsystemBodies m_subsystemBodies;
        readonly SubsystemProjectiles m_subsystemProjectiles;
        readonly SubsystemPickables m_subsystemPickables;

        public ASMPortal LinkedPortal;

        //传送所需
        public Dictionary<ComponentBody, Vector3> m_needTeleportBodies = new Dictionary<ComponentBody, Vector3>();
        public Dictionary<Projectile, Vector3> m_needTeleportProjectiles = new Dictionary<Projectile, Vector3>();
        public Dictionary<Pickable, Vector3> m_needTeleportPickables = new Dictionary<Pickable, Vector3>();

        public ASMPortal(ASMComplexPerspectiveCamera camera, GameWidget gameWidget, Matrix transformMatrix, int screenUVSubdivision, Project project, ComponentASMPortal controller, ComponentPlayer player) {
            m_camera = camera;
            m_gameWidget = gameWidget;
            m_transformMatrix = transformMatrix;
            m_screenUVSubdivision = screenUVSubdivision;
            m_controller = controller;
            m_player = player;
            Project = project;
            m_subsystemTerrain = project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemCameraGameWidgets = project.FindSubsystem<SubsystemASMCamerasGameWidgets>(true);
            m_subsystemGameWidgets = project.FindSubsystem<SubsystemGameWidgets>(true);
            m_subsystemSky = project.FindSubsystem<SubsystemSky>(true);
            m_subsystemDrawing = project.FindSubsystem<SubsystemDrawing>(true);
            m_subsystemBodies = project.FindSubsystem<SubsystemBodies>(true);
            m_subsystemProjectiles = project.FindSubsystem<SubsystemProjectiles>(true);
            m_subsystemPickables = project.FindSubsystem<SubsystemPickables>(true);
        }

        public void LinkPortal(ASMPortal portal) {
            LinkedPortal = portal;
        }

        public void SetTransformMatrix(Matrix transformMatrix) => m_transformMatrix = transformMatrix;

        public void DrawView(PrimitivesRenderer3D primitivesRenderer3D) {
            m_camera.PrepareForDrawing();
            Vector3 translation = m_camera.ViewMatrix.Translation;
            if ((m_lastViewTranslation.XZ - translation.XZ).Length() > 16) {
                m_lastViewTranslation = translation;
                m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(m_camera.GameWidget.PlayerData.PlayerIndex, translation.XZ, MathUtils.Min(m_subsystemTerrain.m_subsystemsky.VisibilityRange, 64f), 64f);
            }

            //去雾
            foreach (TerrainChunk terrainChunk in m_subsystemTerrain.Terrain.AllocatedChunks) {
                if (Vector2.DistanceSquared(m_camera.ViewPosition.XZ, terrainChunk.Center) <= MathUtils.Sqr(m_subsystemSky.VisibilityRange)
                    && terrainChunk.State == TerrainChunkState.Valid) {
                    terrainChunk.FogEnds[0] = float.MaxValue;
                }
            }
            RenderTarget2D lastRenderTarget = Display.RenderTarget;
            Display.RenderTarget = m_camera.ViewTexture;
            Display.Clear(Color.Black, 1f, 0);
            try {
                m_subsystemDrawing.Draw(m_camera);
            }
            finally {
                Display.RenderTarget = lastRenderTarget;
            }
        }

        public void DrawPortal(PrimitivesRenderer3D primitivesRenderer3D, Camera camera) {
            if (LinkedPortal == null) return;
            if (camera == m_camera)
                return;
            DrawScreen(primitivesRenderer3D, camera);
            //传送逻辑
            if(m_teleportEnable) UpdateTeleport();
            UpdateOffsetY();
        }

        public void Update(float dt) {
            if(LinkedPortal == null) return;

            Matrix playerView = m_player.GameWidget.ActiveCamera.ViewMatrix.Invert();
            Matrix portal1Trans = TransformMatrixWithoutScale;
            Matrix portal2Trans = LinkedPortal.TransformMatrixWithoutScale;

            //绘制画面所需
            Matrix playerCamToPortal2 = playerView * portal2Trans.Invert();
            Matrix viewMatrix1 = playerCamToPortal2 * portal1Trans;
            m_camera.SetViewMatrix(viewMatrix1);
            m_camera.m_projectionMatrix = m_player.GameWidget.ActiveCamera.ProjectionMatrix;
        }

        public void UpdateTeleport() {
            Matrix portal1Trans = TransformMatrixWithoutScale;
            Matrix portal2Trans = LinkedPortal.TransformMatrixWithoutScale;
            //传送玩家所需
            CalcBoundaries(out Vector3 p1, out Vector3 p2, out Vector3 p3, out Vector3 p4, out Vector3 portalCenter);//传送门屏幕的四个边界点以及一个中心点
            Vector3 right = (p2 - p1).Normalize();
            Vector3 forward = (p4 - p1).Normalize();
            Vector3 up = Vector3.Cross(right, forward).Normalize();//传送门屏幕的坐标系
            DynamicArray<ComponentBody> foundBodies = new DynamicArray<ComponentBody>();
            m_subsystemBodies.FindBodiesInArea((p1 - up).XZ, (p3 + up).XZ, foundBodies);
            List<Projectile> foundProjectiles = FindProjectiles();
            List<Pickable> foundPickables = FindPickables();

            //实体
            foreach (var foundBody in foundBodies) {
                Vector3 bodyPosition = foundBody.Position + foundBody.BoundingBox.Size().Y / 2 * Vector3.UnitY;
                if (!m_needTeleportBodies.ContainsKey(foundBody)
                    && IsInPortalField(
                        p1,
                        p2,
                        p3,
                        p4,
                        0.5f,
                        bodyPosition
                    ))
                    m_needTeleportBodies.Add(foundBody, bodyPosition); //待传送的实体里不包含检测到的实体，则作为新实体加入
            }
            for (int i = 0; i < m_needTeleportBodies.Keys.Count; i++) {
                ComponentBody needTeleportBody = m_needTeleportBodies.Keys.ToList()[i];
                Vector3 bodyPosition = needTeleportBody.Position + needTeleportBody.BoundingBox.Size().Y / 2 * Vector3.UnitY;
                if (!foundBodies.Contains(needTeleportBody)
                    || !IsInPortalField(
                        p1,
                        p2,
                        p3,
                        p4,
                        0.5f,
                        bodyPosition
                    )) {
                    m_needTeleportBodies.Remove(needTeleportBody); //待传送的实体内包含多余的实体，则删除
                    continue;
                }
                //传送逻辑
                Vector3 oldBodyPosition = m_needTeleportBodies[needTeleportBody];
                if (oldBodyPosition != bodyPosition) {
                    Vector3 offsetFromPortal = bodyPosition - portalCenter;
                    Vector3 oldOffsetFromPortal = oldBodyPosition - portalCenter;
                    int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, up));//当前帧实体所在传送门的哪一边
                    int oldPortalSide = Math.Sign(Vector3.Dot(oldOffsetFromPortal, up));//上一帧在传送门的哪一边
                    if (portalSide != oldPortalSide) {//玩家穿过了传送门, 执行传送逻辑
                        Matrix bodyToPortal1 = needTeleportBody.Matrix * portal1Trans.Invert();
                        Matrix bodyMatrix2 = bodyToPortal1 * portal2Trans;
                        bodyMatrix2.Decompose(out _, out Quaternion rotation, out Vector3 position);
                        needTeleportBody.Position = position;
                        needTeleportBody.Rotation = rotation;
                        needTeleportBody.Velocity = Vector3.Transform(needTeleportBody.Velocity, (portal1Trans.Invert() * portal2Trans).OrientationMatrix);
                        m_needTeleportBodies.Remove(needTeleportBody);
                        i--;
                        LinkedPortal.UpdateOffsetY();
                    }
                    else {
                        m_needTeleportBodies[needTeleportBody] = bodyPosition;
                    }
                }
            }

            //投掷物
            foreach (var foundProjectile in foundProjectiles) {
                if (!m_needTeleportProjectiles.ContainsKey(foundProjectile)) m_needTeleportProjectiles.Add(foundProjectile, foundProjectile.Position);//待传送的实体里不包含检测到的实体，则作为新实体加入
            }
            for (int i = 0; i < m_needTeleportProjectiles.Keys.Count; i++) {
                Projectile needTeleportProjectile = m_needTeleportProjectiles.Keys.ToList()[i];
                if (!foundProjectiles.Contains(needTeleportProjectile)) {
                    m_needTeleportProjectiles.Remove(needTeleportProjectile);
                    continue;
                }
                //传送逻辑
                Vector3 projectilePosition = needTeleportProjectile.Position;
                Vector3 oldProjectilePosition = m_needTeleportProjectiles[needTeleportProjectile];
                if (oldProjectilePosition != projectilePosition) {
                    Vector3 offsetFromPortal = projectilePosition - portalCenter;
                    Vector3 oldOffsetFromPortal = oldProjectilePosition - portalCenter;
                    int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, up));//当前帧实体所在传送门的哪一边
                    int oldPortalSide = Math.Sign(Vector3.Dot(oldOffsetFromPortal, up));//上一帧在传送门的哪一边
                    if (portalSide != oldPortalSide) {//玩家穿过了传送门, 执行传送逻辑
                        Matrix projectileMatrix = Matrix.CreateFromYawPitchRoll(needTeleportProjectile.Rotation.X, needTeleportProjectile.Rotation.Y, needTeleportProjectile.Rotation.Z) * Matrix.CreateTranslation(needTeleportProjectile.Position);
                        Matrix projectileToPortal1 = projectileMatrix * portal1Trans.Invert();
                        Matrix projectileMatrix2 = projectileToPortal1 * portal2Trans;
                        projectileMatrix2.Decompose(out _, out _, out Vector3 position);
                        needTeleportProjectile.Position = position;
                        needTeleportProjectile.Rotation = projectileMatrix2.ToYawPitchRoll();
                        needTeleportProjectile.Velocity = Vector3.Transform(needTeleportProjectile.Velocity, (portal1Trans.Invert() * portal2Trans).OrientationMatrix);
                        needTeleportProjectile.AngularVelocity = Vector3.Transform(needTeleportProjectile.AngularVelocity, (portal1Trans.Invert() * portal2Trans).OrientationMatrix);
                        m_needTeleportProjectiles.Remove(needTeleportProjectile);
                        i--;
                    }
                    else {
                        m_needTeleportProjectiles[needTeleportProjectile] = projectilePosition;
                    }
                }
            }

            //掉落物
            foreach (var foundPickable in foundPickables) {
                if (!m_needTeleportPickables.ContainsKey(foundPickable)) m_needTeleportPickables.Add(foundPickable, foundPickable.Position);//待传送的实体里不包含检测到的实体，则作为新实体加入
            }
            for (int i = 0; i < m_needTeleportPickables.Keys.Count; i++) {
                Pickable needTeleportPickable = m_needTeleportPickables.Keys.ToList()[i];
                if (!foundPickables.Contains(needTeleportPickable)) {
                    m_needTeleportPickables.Remove(needTeleportPickable);
                    continue;
                }
                //传送逻辑
                Vector3 pickablePosition = needTeleportPickable.Position;
                Vector3 oldPickablePosition = m_needTeleportPickables[needTeleportPickable];
                if (oldPickablePosition != pickablePosition) {
                    Vector3 offsetFromPortal = pickablePosition - portalCenter;
                    Vector3 oldOffsetFromPortal = oldPickablePosition - portalCenter;
                    int portalSide = Math.Sign(Vector3.Dot(offsetFromPortal, up));//当前帧实体所在传送门的哪一边
                    int oldPortalSide = Math.Sign(Vector3.Dot(oldOffsetFromPortal, up));//上一帧在传送门的哪一边
                    if (portalSide != oldPortalSide) {//玩家穿过了传送门, 执行传送逻辑
                        Matrix pickableMatrix = Matrix.CreateTranslation(needTeleportPickable.Position);
                        Matrix pickableToPortal1 = pickableMatrix * portal1Trans.Invert();
                        Matrix pickableMatrix2 = pickableToPortal1 * portal2Trans;
                        pickableMatrix2.Decompose(out _, out _, out Vector3 position);
                        needTeleportPickable.Position = position;
                        needTeleportPickable.Velocity = Vector3.Transform(needTeleportPickable.Velocity, (portal1Trans.Invert() * portal2Trans).OrientationMatrix);
                        m_needTeleportPickables.Remove(needTeleportPickable);
                        i--;
                    }
                    else {
                        m_needTeleportPickables[needTeleportPickable] = pickablePosition;
                    }
                }
            }
        }

        public void UpdateOffsetY() {
                         //计算玩家摄像机裁剪矩形到传送门屏幕的距离，根据距离偏移传送门防止视角露馅
            m_offsetY = 0;
            CalcBoundaries(out Vector3 p1, out Vector3 p2, out Vector3 p3, out Vector3 p4, out Vector3 portalCenter);
            if (IsInPortalField(
                    p1,
                    p2,
                    p3,
                    p4,
                    0.5f,
                    m_player.GameWidget.ActiveCamera.ViewMatrix.Invert().Translation
                )) {
                Vector3 portalNormal = Vector3.Cross(p2 - p1, p4 - p1).Normalize();//传送门屏幕法向量
                BoundingFrustum playerCamFrustum = m_player.GameWidget.ActiveCamera.ViewFrustum;
                Vector3[] frustumCorners = playerCamFrustum.FindCorners();
                Vector3 playerCamTrans = m_player.GameWidget.ActiveCamera.ViewMatrix.Invert().Translation;
                Vector3 playerCamBottom0 = frustumCorners[0];
                Vector3 playerCamBottom1 = frustumCorners[1];
                Vector3 playerCamBottom2 = frustumCorners[2];
                Vector3 playerCamBottom3 = frustumCorners[3];
                int camViewSide = (int)MathUtils.Sign(Vector3.Dot((playerCamTrans - portalCenter), portalNormal));
                int camBottom1Side = (int)MathUtils.Sign(Vector3.Dot((playerCamBottom0 - portalCenter), portalNormal));
                int camBottom2Side = (int)MathUtils.Sign(Vector3.Dot((playerCamBottom1 - portalCenter), portalNormal));
                int camBottom3Side = (int)MathUtils.Sign(Vector3.Dot((playerCamBottom2 - portalCenter), portalNormal));
                int camBottom4Side = (int)MathUtils.Sign(Vector3.Dot((playerCamBottom3 - portalCenter), portalNormal));
                float camB1ToPortalL = CalcPointToPlaneDistance(p1, p2, p3, p4, playerCamBottom0) * camViewSide;
                float camB2ToPortalL = CalcPointToPlaneDistance(p1, p2, p3, p4, playerCamBottom1) * camViewSide;
                float camB3ToPortalL = CalcPointToPlaneDistance(p1, p2, p3, p4, playerCamBottom2) * camViewSide;
                float camB4ToPortalL = CalcPointToPlaneDistance(p1, p2, p3, p4, playerCamBottom3) * camViewSide;
                Vector3 camBCenter = (playerCamBottom0 + playerCamBottom1 + playerCamBottom2 + playerCamBottom3) / 4;
                float velocityToNormal = MathUtils.Abs(Vector3.Dot(m_player.ComponentBody.Velocity, portalNormal));//速度投影在法向量上的值
                if (camBottom1Side != camViewSide || camBottom2Side != camViewSide || camBottom3Side != camViewSide || camBottom4Side != camViewSide) {//如果不等于，则表明摄像机近裁剪平面已经穿过传送门而摄像机根部未穿过
                    m_offsetY = -camViewSide * (MathUtils.Min(camB1ToPortalL, camB2ToPortalL, camB3ToPortalL, camB4ToPortalL) - (playerCamTrans - camBCenter).Length());
                }
            }
        }

        public void DrawScreen(PrimitivesRenderer3D primitivesRenderer3D, Camera camera) {
                        //传送门1
            for (int i = 0; i < m_screenUVSubdivision; i++) {
                for (int j = 0; j < m_screenUVSubdivision; j++) {
                    float length = 1 / (float)m_screenUVSubdivision;
                    Vector3 wp1 = Vector3.Transform(new Vector3(length * i - 0.5f, m_offsetY, length * j - 0.5f), m_transformMatrix);
                    Vector3 wp2 = Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, m_offsetY, length * j - 0.5f), m_transformMatrix);
                    Vector3 wp3 = Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, m_offsetY, length * (j + 1) - 0.5f), m_transformMatrix);
                    Vector3 wp4 = Vector3.Transform(new Vector3(length * i - 0.5f, m_offsetY, length * (j + 1) - 0.5f), m_transformMatrix);
                    if (!IsInPlayerFrustum(camera, wp1, wp2, wp3, wp4)) continue;
                    Vector3 sp1 = camera.WorldToScreen(wp1, Matrix.Identity);
                    Vector3 sp2 = camera.WorldToScreen(wp2, Matrix.Identity);
                    Vector3 sp3 = camera.WorldToScreen(wp3, Matrix.Identity);
                    Vector3 sp4 = camera.WorldToScreen(wp4, Matrix.Identity);
                    Vector2 sp11 = new Vector2(sp1.X, sp1.Y);
                    Vector2 sp21 = new Vector2(sp2.X, sp2.Y);
                    Vector2 sp31 = new Vector2(sp3.X, sp3.Y);
                    Vector2 sp41 = new Vector2(sp4.X, sp4.Y);
                    sp11 = new Vector2(sp11.X / camera.ViewportSize.X, sp11.Y / camera.ViewportSize.Y);
                    sp21 = new Vector2(sp21.X / camera.ViewportSize.X, sp21.Y / camera.ViewportSize.Y);
                    sp31 = new Vector2(sp31.X / camera.ViewportSize.X, sp31.Y / camera.ViewportSize.Y);
                    sp41 = new Vector2(sp41.X / camera.ViewportSize.X, sp41.Y / camera.ViewportSize.Y);
                    primitivesRenderer3D.TexturedBatch(
                            LinkedPortal.m_camera.ViewTexture, //传送门1显示传送门2对应的摄像机的画面
                            useAlphaTest: true,
                            0,
                            null,
                            RasterizerState.CullNone,
                            null,
                            SamplerState.LinearWrap
                        )
                        .QueueQuad(
                            wp1,
                            wp2,
                            wp3,
                            wp4,
                            sp11,
                            sp21,
                            sp31,
                            sp41,
                            Color.White
                        );
                }
            }
        }

        public void Dispose() {
            m_subsystemTerrain.TerrainUpdater.RemoveUpdateLocation(m_gameWidget.PlayerData.PlayerIndex);
            m_subsystemGameWidgets.m_gameWidgets.Remove(m_gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Remove(m_gameWidget);
            m_gameWidget.Dispose();
            m_gameWidget = null;
            Utilities.Dispose(ref m_camera.ViewTexture);
            m_gameWidget = null;
        }

        public bool IsInPlayerFrustum(Camera camera, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
            BoundingFrustum frustum = camera.ViewFrustum;
            return frustum.Intersection(p1) || frustum.Intersection(p2) || frustum.Intersection(p3) || frustum.Intersection(p4);
        }

        public void CalcBoundaries(out Vector3 p1, out Vector3 p2, out Vector3 p3, out Vector3 p4, out Vector3 center) {
            p1 = Vector3.Transform(new Vector3(-0.5f, 0, -0.5f), m_transformMatrix);
            p2 = Vector3.Transform(new Vector3(0.5f, 0, -0.5f), m_transformMatrix);
            p3 = Vector3.Transform(new Vector3(0.5f, 0, 0.5f), m_transformMatrix);
            p4 = Vector3.Transform(new Vector3(-0.5f, 0, 0.5f), m_transformMatrix);
            center = Vector3.Transform(new Vector3(0, 0, 0), m_transformMatrix);
        }

        public float CalcPointToPlaneDistance(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p) {
            Vector3 normal = Vector3.Cross(p2 - p1, p4 - p1);
            float distance = Vector3.Dot(p - p1, normal) / normal.Length();
            return distance;
        }

        public bool IsInPortalField(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float depth, Vector3 p) {
            Vector3 normal = Vector3.Cross(b - a, d - a);
            float distance = Vector3.Dot(p - a, normal) / normal.Length();
            if (float.IsNaN(distance)) return false;
            if (MathUtils.Abs(distance) > depth) return false;//检测区域的深度
            Vector3 projP = p - normal * distance;
            float factor1 = Vector3.Dot(normal, Vector3.Cross(b - projP, c - projP));
            float factor2 = Vector3.Dot(normal, Vector3.Cross(c - projP, d - projP));
            float factor3 = Vector3.Dot(normal, Vector3.Cross(d - projP, a - projP));
            float factor4 = Vector3.Dot(normal, Vector3.Cross(a - projP, b - projP));
            return MathUtils.Sign(factor1) == MathUtils.Sign(factor2) && MathUtils.Sign(factor3) == MathUtils.Sign(factor4) && MathUtils.Sign(factor2) == MathUtils.Sign(factor3);
        }

        public List<Projectile> FindProjectiles() {
            List<Projectile> projectiles = new List<Projectile>();
            CalcBoundaries(out Vector3 p1, out Vector3 p2, out Vector3 p3, out Vector3 p4, out Vector3 center);
            Vector3 normal = Vector3.Cross(p2 - p1, p4 - p1);
            foreach (var projectile in m_subsystemProjectiles.Projectiles) {
                if (IsInPortalField(
                        p1,
                        p2,
                        p3,
                        p4,
                        0.5f,
                        projectile.Position
                    )
                    && Vector3.Distance(projectile.Position, center) < normal.Length()) {
                    projectiles.Add(projectile);
                }
            }
            return projectiles;
        }

        public List<Pickable> FindPickables() {
            List<Pickable> pickables = new List<Pickable>();
            CalcBoundaries(out Vector3 p1, out Vector3 p2, out Vector3 p3, out Vector3 p4, out Vector3 center);
            Vector3 normal = Vector3.Cross(p2 - p1, p4 - p1);
            foreach (var pickable in m_subsystemPickables.Pickables) {
                if (IsInPortalField(
                        p1,
                        p2,
                        p3,
                        p4,
                        0.5f,
                        pickable.Position
                    )
                    && Vector3.Distance(pickable.Position, center) < normal.Length()) {
                    pickables.Add(pickable);
                }
            }
            return pickables;
        }
    }
}