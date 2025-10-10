using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class ChenCelikovskyAttractor : GH_Component
    {
        public ChenCelikovskyAttractor()
          : base("ChenCelikovskyAttractor", "AA",
            "Construct a ChenCelikovskyAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(1, 1, 1));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 36);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, 3);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, 20);
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
            double Delta = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Alpha)) return;
            if (!DA.GetData(2, ref Beta)) return;
            if (!DA.GetData(3, ref Delta)) return;
            if (!DA.GetData(4, ref DeltaT)) return;
            if (!DA.GetData(5, ref Iterations)) return;

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
            List<Point3d> ChenCelikovskyAttractorPoints = GenerateChenCelikovskyAttractor(StartPoint, Alpha, Beta, Delta,  DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)ChenCelikovskyAttractorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(ChenCelikovskyAttractorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateChenCelikovskyAttractor(Point3d StartPoint, double Alpha, double Beta,  double Delta, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = Alpha*(y-x);
                double dy = -x*z+Delta*y;
                double dz = x*y-Beta*z;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("2D7E9A4C-1F3B-4A8D-9C6E-5B3A7D1F4E8C");
    }
}