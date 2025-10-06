using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class ChuaAttractor : GH_Component
    {
        public ChuaAttractor()
          : base("ChuaAttractor", "CA",
            "Construct a Chua's Attractor",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("StartPoint", "P", "Start Point", GH_ParamAccess.item, new Point3d(0.1, 0, 0));
            pManager.AddNumberParameter("Alpha", "¦Á", "Alpha parameter", GH_ParamAccess.item, 15.6);
            pManager.AddNumberParameter("Beta", "¦Â", "Beta parameter", GH_ParamAccess.item, 28.0);
            pManager.AddNumberParameter("M0", "M0", "M0 parameter for piecewise linear function", GH_ParamAccess.item, -1.143);
            pManager.AddNumberParameter("M1", "M1", "M1 parameter for piecewise linear function", GH_ParamAccess.item, -0.714);
            pManager.AddNumberParameter("DeltaT", "¦¤t", "Time step", GH_ParamAccess.item, 0.01);
            pManager.AddIntegerParameter("Iterations", "I", "Number of iterations", GH_ParamAccess.item, 10000);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Chua Attractor Points", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "C", "Chua Attractor Curve", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Read input parameters
            Point3d startPoint = Point3d.Origin;
            double alpha = 15.6;
            double beta = 28.0;
            double m0 = -1.143;
            double m1 = -0.714;
            double deltaT = 0.01;
            int iterations = 10000;

            if (!DA.GetData(0, ref startPoint)) return;
            if (!DA.GetData(1, ref alpha)) return;
            if (!DA.GetData(2, ref beta)) return;
            if (!DA.GetData(3, ref m0)) return;
            if (!DA.GetData(4, ref m1)) return;
            if (!DA.GetData(5, ref deltaT)) return;
            if (!DA.GetData(6, ref iterations)) return;

            // Validate parameters
            if (deltaT <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "DeltaT must be positive");
                return;
            }

            if (iterations <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Iterations must be positive");
                return;
            }

            // Generate Chua attractor points
            List<Point3d> chuaPoints = GenerateChuaAttractor(startPoint, alpha, beta, m0, m1, deltaT, iterations);
            
            // Set outputs
            DA.SetDataList(0, chuaPoints);

        }

        private List<Point3d> GenerateChuaAttractor(Point3d startPoint, double alpha, double beta, double m0, double m1, double deltaT, int iterations)
        {
            List<Point3d> points = new List<Point3d>();
            
            double x = startPoint.X;
            double y = startPoint.Y;
            double z = startPoint.Z;

            // Add initial point
            points.Add(new Point3d(x, y, z));

            for (int i = 0; i < iterations; i++)
            {
                // Chua's circuit equations:
                // dx/dt = ¦Á(y - x - f(x))
                // dy/dt = x - y + z  
                // dz/dt = -¦Ây
                
                // Piecewise linear function f(x)
                double fx = m1 * x + 0.5 * (m0 - m1) * (Math.Abs(x + 1) - Math.Abs(x - 1));
                
                // Calculate derivatives
                double dx = alpha * (y - x - fx);
                double dy = x - y + z;
                double dz = -beta * y;
                
                // Update using Euler method
                x += dx * deltaT;
                y += dy * deltaT;
                z += dz * deltaT;
                
                points.Add(new Point3d(x, y, z));
            }

            return points;
        }

        // Optional: Improved version using Runge-Kutta 4th order method



        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("7A3B8C21-45BB-4F95-A22C-B92CFD4479F3");
    }
}