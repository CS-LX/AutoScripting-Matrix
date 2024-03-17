using Engine;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMDisplayMatrix : Component, IUpdateable, IDrawable {
        public UpdateOrder UpdateOrder => UpdateOrder.Default;
        public int[] DrawOrders => [1];

        public ComponentPlayer m_componentPlayer;

        public ComponentBlockHighlight m_componentBlockHighlight;

        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemASMElectricity m_m_subsystemASMElectricity;

        public SubsystemASMatrixDisplay m_subsystemASMatrixDisplay;

        public ASMatrixDisplayData[] m_displays = new ASMatrixDisplayData[4];

        public void Draw(Camera camera, int drawOrder) {
            if ((bool)ASMSettingsManager.Get("DisplayConnectorMatrix")
                && camera.GameWidget.PlayerData == m_componentPlayer.PlayerData
                && drawOrder == DrawOrders[0]
                && !camera.UsesMovementControls
                && m_componentPlayer.ComponentHealth.Health > 0
                && m_componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible) {
                if (m_componentPlayer.ComponentMiner.DigCellFace.HasValue) {
                    DisplayMatrix(m_componentPlayer.ComponentMiner.DigCellFace.Value);
                }
                else if (!m_componentPlayer.ComponentAimingSights.IsSightsVisible
                    && m_componentBlockHighlight.m_highlightRaycastResult is TerrainRaycastResult result) {
                    DisplayMatrix(result.CellFace);
                }
            }
        }

        public void Update(float dt) { }

        public void DisplayMatrix(CellFace cellFace) {
            foreach (var displayData in m_displays) {
                m_subsystemASMatrixDisplay.SetVisible(displayData, false);
            }
            
            int blockValue = m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
            int blockContents = Terrain.ExtractContents(blockValue);
            int blockData = Terrain.ExtractData(blockValue);
            int blockFace = 4;
            Block block = BlocksManager.Blocks[blockContents];
            switch (block) {
                case ASMMountedElectricElementBlock mountedBlock:
                    blockFace = mountedBlock.GetFace(blockValue);
                    if (m_m_subsystemASMElectricity.m_electricElementsByCellFace.TryGetValue(new CellFace(cellFace.X, cellFace.Y, cellFace.Z, blockFace), out ASMElectricElement element)) {
                        //计算面本地单位坐标
                        Vector3 position = new Vector3(cellFace.X + 0.5f, cellFace.Y + 0.5f, cellFace.Z + 0.5f) - 0.3f * CellFace.FaceToVector3(blockFace);
                        Vector3 forward = CellFace.FaceToVector3(blockFace);//面的法向
                        int rotation = 0;
                        if (mountedBlock is ASMRotateableMountedElectricElementBlock) {
                            rotation = ASMRotateableMountedElectricElementBlock.GetRotation(blockData);
                        }
                        Vector3 up = blockFace < 4 ? Vector3.UnitY : rotation switch {
                            1 => Vector3.UnitX,
                            2 => Vector3.UnitZ,
                            3 => -Vector3.UnitX,
                            _ => -Vector3.UnitZ
                        };
                        Vector3 right = Vector3.Cross(forward, up);
                        //处理面
                        int[] faces = ASMUtils.NormalToFaces(forward, 0.1f);
                        //显示矩阵
                        for (int i = 0; i < 4; i++) {
                            //获取端口
                            ASMElectricConnectorType? connectorType = mountedBlock.GetConnectorType(
                                m_subsystemTerrain,
                                blockValue,
                                blockFace,
                                faces[i],
                                cellFace.X,
                                cellFace.Y,
                                cellFace.Z
                            );
                            if (connectorType == null) continue;
                            ASMElectricConnection connection = element.Connections.Find(connection => connection.ConnectorFace == faces[i]);


                            Vector2 offset = Vector2.One * (0.5f - 1f / 5);
                            Vector3 offset3 = CellFace.FaceToVector3(faces[i]) * 0.5f;

                            m_subsystemASMatrixDisplay.SetVisible(m_displays[i], true);
                            m_displays[i].DisplayPoint = new CellFace(cellFace.X, cellFace.Y, cellFace.Z, blockFace);
                            m_displays[i].Height = 2f / 5;
                            m_displays[i].Width = 2f / 5;
                            m_displays[i].DisplayType = ASMatrixDisplayType.RowLines | ASMatrixDisplayType.ColumnLines;
                            m_displays[i].Offset = offset;
                            m_displays[i].Offset3 = offset3;
                            m_displays[i].UseDebugFont = true;
                            m_displays[i].FontScale = 0.7f;
                            m_displays[i].RowLinesWidth = m_displays[i].ColumnLinesWidth = 0.006f;
                            m_displays[i].RowLinesColor = m_displays[i].ColumnLinesColor = Color.White * (connectorType.Value == ASMElectricConnectorType.Output ? SubsystemASMManager.OutputColor : SubsystemASMManager.InputColor);

                            //获取电压
                            Matrix? voltage = connectorType == ASMElectricConnectorType.Output ? element.GetOutputVoltage(faces[i]) : connection?.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                            m_displays[i].Matrix = voltage.HasValue ? voltage.Value : Matrix.Zero;
                        }
                    }
                    break;
            }
        }

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_componentPlayer = Entity.FindComponent<ComponentPlayer>(true);
            m_componentBlockHighlight = Entity.FindComponent<ComponentBlockHighlight>(true);
            m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemASMatrixDisplay = Project.FindSubsystem<SubsystemASMatrixDisplay>(true);
            m_m_subsystemASMElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            for (int i = 0; i < 4; i++) {
                ASMatrixDisplayData asMatrixDisplayData = m_subsystemASMatrixDisplay.Add(false);
                m_displays[i] = asMatrixDisplayData;
            }
        }
    }
}