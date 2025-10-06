using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class LorenzStenfloAttractor : GH_Component
    {
        public LorenzStenfloAttractor()
          : base("LorenzStenfloAttractor", "AA",
            "Construct a LorenzStenfloAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(0.1, 0.1, 0.2));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 2);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, 0.7);
            pManager.AddNumberParameter("Zeta", "ζ", "Zeta", GH_ParamAccess.item, 26);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, 1.5);
            pManager.AddNumberParameter("DeltaT", "Δt", "DeltaT", GH_ParamAccess.item, 0.001);
            pManager.AddIntegerParameter("Iterations", "I", "Number of  iterations", GH_ParamAccess.item, 10000);

        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddPointParameter("Points", "P", "LorenzOscillator", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "C", "LorenzOscillator", GH_ParamAccess.item);

            //pManager.HideParameter(0);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Point3d StartPoint = Point3d.Origin;
            double Alpha = 0.0;
            double Beta = 0.0;
            double Zeta = 0.0;
            double Delta = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Alpha)) return;
            if (!DA.GetData(2, ref Beta)) return;
            if (!DA.GetData(3, ref Zeta)) return;
            if (!DA.GetData(4, ref Delta)) return;
            if (!DA.GetData(5, ref DeltaT)) return;
            if (!DA.GetData(6, ref Iterations)) return;

            if (DeltaT <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "DeltaT must be positive");
                return;
            }

            if (Iterations <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Iterations must be positive");
                return;
            }
            List<Point3d> LorenzStenfloAttractorPoints = GenerateLorenzStenfloAttractor(StartPoint, Alpha, Beta, Zeta, Delta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)LorenzStenfloAttractorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(LorenzStenfloAttractorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateLorenzStenfloAttractor(Point3d StartPoint, double Alpha, double Beta, double Zeta, double Delta, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;
            double w = 0.3;


            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = Alpha*(y-x)+Delta*w;
                double dy = x*(Zeta-z)-y;
                double dz = x*y-Beta*z;
                double dw = -x-Alpha*w;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;
                w += dw * DeltaT;
                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("6E7D9A4C-1F3B-4A8D-9C6E-5B3A7D1F4E8C");
    }
}