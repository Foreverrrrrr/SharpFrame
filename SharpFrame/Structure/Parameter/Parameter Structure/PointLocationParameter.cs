namespace SharpFrame.Structure.Parameter
{
    /// <summary>
    /// 点位参数结构
    /// </summary>
    public class PointLocationParameter
    {
        public PointLocationParameter(PointLocationParameter system)
        {
            if (system != null)
            {
                this.ID = system.ID;
                this.Name = system.Name;
                this.Enable = system.Enable;
                this.PointA = system.PointA;
                this.PointB = system.PointB;
                this.PointC = system.PointC;
                this.PointD = system.PointD;
                this.PointE = system.PointE;
                this.PointF = system.PointF;
            }
        }

        public PointLocationParameter() { }
        public int ID { get; set; }

        public string Name { get; set; }

        public bool Enable { get; set; }

        public double PointA { get; set; }

        public double PointB { get; set; }

        public double PointC { get; set; }

        public double PointD { get; set; }

        public double PointE { get; set; }

        public double PointF { get; set; }
    }
}
