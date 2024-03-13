using Engine;
using System.Collections.Generic;

namespace Game {
    public class ASMDispenserElectricElement : ASMElectricElement {

        public double? m_lastDispenseTime;

        public SubsystemBlockEntities m_subsystemBlockEntities;

        public Matrix m_voltage;

        public ASMDispenserElectricElement(SubsystemASMElectricity subsystemElectricity, Point3 point) : base(
            subsystemElectricity,
            new List<CellFace> {
                new CellFace(point.X, point.Y, point.Z, 0),
                new CellFace(point.X, point.Y, point.Z, 1),
                new CellFace(point.X, point.Y, point.Z, 2),
                new CellFace(point.X, point.Y, point.Z, 3),
                new CellFace(point.X, point.Y, point.Z, 4),
                new CellFace(point.X, point.Y, point.Z, 5)
            }
        ) {
            m_subsystemBlockEntities = SubsystemElectricity.Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true);
            Matrix? volatge = SubsystemElectricity.ReadPersistentVoltage(CellFaces[0].Point); //读取存着的电压
            m_voltage = volatge.HasValue ? volatge.Value : Matrix.Zero;
        }

        public override bool Simulate() {
            Matrix voltage = Matrix.Zero;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    voltage += connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                }
            }
            if (voltage != m_voltage) {
                m_voltage = voltage;
                SubsystemElectricity.WritePersistentVoltage(CellFaces[0].Point, m_voltage);
                if ((!m_lastDispenseTime.HasValue || SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)) {
                    if (voltage != Matrix.Zero) {
                        m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
                        m_subsystemBlockEntities.GetBlockEntity(CellFaces[0].Point.X, CellFaces[0].Point.Y, CellFaces[0].Point.Z)?.Entity.FindComponent<ComponentASMDispenser>()?.Dispense(m_voltage);
                    }
                }
            }
            return false;
        }
    }
}