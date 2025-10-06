using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class LiuChenAttractor : GH_Component
    {
        public LiuChenAttractor()
          : base("LiuChenAttractor", "AA",
            "Construct a LiuChenAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(0.1, 0.1, 0.02));
            pManager.AddNumberParameter("Alpha", "α", "Alpha", GH_ParamAccess.item, 2.4);
            pManager.AddNumberParameter("Beta", "β", "Beta", GH_ParamAccess.item, -3.78);
            pManager.AddNumberParameter("Gama", "γ", "Gama", GH_ParamAccess.item, -14);
            pManager.AddNumberParameter("Delta", "δ", "Delta", GH_ParamAccess.item, -11);
            pManager.AddNumberParameter("Epsilon", "ε", "Epsilon", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("Zeta", "ζ", "Zeta", GH_ParamAccess.item, 5.58);
            pManager.AddNumberParameter("Rou", "ρ", "Rou", GH_ParamAccess.item, 1);
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
            double Gama = 0.0;
            double Delta = 0.0;
            double Epsilon = 0.0;
            double Zeta = 0.0;
            double Rou = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Alpha)) return;
            if (!DA.GetData(2, ref Beta)) return;
            if (!DA.GetData(3, ref Gama)) return;
            if (!DA.GetData(4, ref Delta)) return;
            if (!DA.GetData(5, ref Epsilon)) return;
            if (!DA.GetData(6, ref Zeta)) return;
            if (!DA.GetData(7, ref Rou)) return;
            if (!DA.GetData(8, ref DeltaT)) return;
            if (!DA.GetData(9, ref Iterations)) return;

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
            List<Point3d> LiuChenAttractorPoints = GenerateLiuChenAttractor(StartPoint, Alpha, Beta, Gama, Delta, Epsilon, Zeta, Rou, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)LiuChenAttractorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(LiuChenAttractorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateLiuChenAttractor(Point3d StartPoint, double Alpha, double Beta, double Gama, double Delta, double Epsilon, double Zeta,double Rou ,double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = Alpha*y+Beta*x+Gama*y*z;
                double dy = Delta*y-z+Epsilon*x*z;
                double dz = Zeta*z+Rou*x*y;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("4F9A7C1D-3E8B-4A2C-9D6E-5B1A8C4F7E3D");
    }
}