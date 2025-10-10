using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class ThreeScrollUnifiedChaoyicSystemAttractortsucs2 : GH_Component
    {
        public ThreeScrollUnifiedChaoyicSystemAttractortsucs2()
          : base("ThreeScrollUnifiedChaoyicSystemAttractortsucs2", "AA",
            "Construct a ThreeScrollUnifiedChaoyicSystemAttractortsucs2.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(0.1, 0.1, 0.1));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 40);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, 0.8333);
            pManager.AddNumberParameter("Gama", "γ", "Gama", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Epsilon", "ε", "Epsilon", GH_ParamAccess.item, 0.65);
            pManager.AddNumberParameter("Zeta", "ζ", "Zeta", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("DeltaT", "Δt", "DeltaT", GH_ParamAccess.item, 0.0001);
            pManager.AddIntegerParameter("Iterations", "I", "Number of  iterations", GH_ParamAccess.item, 100000);

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
            List<Point3d> ThreeScrollUnifiedChaoyicSystemAttractortsucs2Points = GenerateThreeScrollUnifiedChaoyicSystemAttractortsucs2(StartPoint, Alpha, Beta, Gama, Delta, Epsilon, Zeta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)ThreeScrollUnifiedChaoyicSystemAttractortsucs2Points;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(ThreeScrollUnifiedChaoyicSystemAttractortsucs2Points, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateThreeScrollUnifiedChaoyicSystemAttractortsucs2(Point3d StartPoint, double Alpha, double Beta, double Gama, double Delta, double Epsilon, double Zeta, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = Alpha * (y - x) + Delta * x * z;
                double dy = Zeta * y - x * z+Gama*y;
                double dz = Beta * z + x * y - Epsilon * x * x;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("4B9A7C1D-3F8E-4A2C-9D6E-5B1A8C4F7E3D");
    }
}