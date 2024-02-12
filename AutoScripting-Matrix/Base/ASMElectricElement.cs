using Engine;

namespace Game {
    public class ASMElectricElement {
        public SubsystemASMElectricity SubsystemElectricity
        {
            get;
            set;
        }

        public ReadOnlyList<CellFace> CellFaces
        {
            get;
            set;
        }

        public List<ASMElectricConnection> Connections
        {
            get;
            set;
        }

        public ASMElectricElement(SubsystemASMElectricity subsystemElectricity, IEnumerable<CellFace> cellFaces)
        {
            SubsystemElectricity = subsystemElectricity;
            CellFaces = new ReadOnlyList<CellFace>(new List<CellFace>(cellFaces));
            Connections = new List<ASMElectricConnection>();
        }

        public ASMElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace)
            : this(subsystemElectricity, new List<CellFace>
            {
                cellFace
            })
        {
        }

        public virtual float GetOutputVoltage(int face)
        {
            return 0f;
        }

        public virtual bool Simulate()
        {
            return false;
        }

        public virtual void OnAdded()
        {
        }

        public virtual void OnRemoved()
        {
        }

        public virtual void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
        {
        }

        public virtual bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            return false;
        }

        public virtual void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
        {
        }

        public virtual void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
        {
        }

        public virtual void OnConnectionsChanged()
        {
        }

        public static bool IsSignalHigh(float voltage)
        {
            return voltage >= 0.5f;
        }

        public int CalculateHighInputsCount()
        {
            int num = 0;
            foreach (ASMElectricConnection connection in Connections)
            {
                if (connection.ConnectorType != ASMElectricConnectorType.Output && connection.NeighborConnectorType != 0 && IsSignalHigh(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace)))
                {
                    num++;
                }
            }
            return num;
        }
    }
}