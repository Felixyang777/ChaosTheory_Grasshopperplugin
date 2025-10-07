using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class WangSunAttractor : GH_Component
    {
        public WangSunAttractor()
          : base("WangSunAttractor", "AA",
            "Construct a WangSunAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(1, 1, 1));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 0.2);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, -0.01);
            pManager.AddNumberParameter("Gama", "γ", "Gama", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, -0.4);
            pManager.AddNumberParameter("Epsilon", "ε", "Epsilon", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("Zeta", "ζ", "Zeta", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("DeltaT", "Δt", "DeltaT", GH_ParamAccess.item, 0.01);
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
            List<Point3d> WangSunAttractorPoints = GenerateWangSunAttractor(StartPoint, Alpha, Beta, Gama, Delta, Epsilon, Zeta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)WangSunAttractorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(WangSunAttractorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateWangSunAttractor(Point3d StartPoint, double Alpha, double Beta, double Gama, double Delta, double Epsilon, double Zeta, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = Alpha * x+Gama*y*z;
                double dy =Beta*x+Delta*y-x*z;
                double dz = Epsilon*z+Zeta*x*y;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("1A7E9B4C-3F8D-4A2C-9D6E-5B1A8C4F7E3D");
    }
}