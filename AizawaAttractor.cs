using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class AizawaAttractor : GH_Component
    {
        public AizawaAttractor()
          : base("AizawaAttractor", "AA",
            "Construct a AizawaAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(1, 0, 0));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 0.95);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, 0.7);
            pManager.AddNumberParameter("Gama", "γ", "Gama", GH_ParamAccess.item, 0.6);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, 3.5);
            pManager.AddNumberParameter("Epsilon", "ε", "Epsilon", GH_ParamAccess.item, 0.25);
            pManager.AddNumberParameter("Zeta", "ζ", "Zeta", GH_ParamAccess.item, 0.1);
            pManager.AddNumberParameter("DeltaT", "Δt", "DeltaT", GH_ParamAccess.item, 0.01);
            pManager.AddIntegerParameter("Iterations", "I", "Number of  iterations", GH_ParamAccess.item, 1000);

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
            double Gama = 0.0;
            double Delta = 0.0;
            double Epsilon = 0.0;
            double Zeta = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Alpha)) return;
            if (!DA.GetData(2, ref Beta)) return;
            if (!DA.GetData(3, ref Gama)) return;
            if (!DA.GetData(4, ref Delta)) return;
            if (!DA.GetData(5, ref Epsilon)) return;
            if (!DA.GetData(6, ref Zeta)) return;
            if (!DA.GetData(7, ref DeltaT)) return;
            if (!DA.GetData(8, ref Iterations)) return;

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
            List<Point3d> AizawaAttractorPoints = GenerateAizawaAttractor(StartPoint, Alpha, Beta, Gama, Delta, Epsilon, Zeta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)AizawaAttractorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(AizawaAttractorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateAizawaAttractor(Point3d StartPoint, double Alpha, double Beta, double Gama, double Delta, double Epsilon, double Zeta, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = (z - Beta) * x - Delta * y;
                double dy = Delta * x + (z - Beta) * y;
                double dz = Gama + Alpha * z - Math.Pow(z, 3) / 3 - (Math.Pow(x, 2) + Math.Pow(y, 2)) * (1 + Epsilon * z) + Zeta * z * Math.Pow(x, 3);

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("8A3B1C9D-4E2F-4A7C-BD82-1E6F5A3C9B7E");
    }
}