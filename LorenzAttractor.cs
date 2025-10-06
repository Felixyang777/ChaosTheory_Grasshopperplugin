using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class LorenzAttractor : GH_Component
    {
        public LorenzAttractor()
          : base("LorenzAttractor", "LA",
            "Construct a LorenzAttractor.",
            "ChaosTheory", "Attractor")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item, new Point3d(1, 0, 0));
            pManager.AddNumberParameter("Sigma", "¦Ò", "Sigma", GH_ParamAccess.item, 10.0);
            pManager.AddNumberParameter("Rou", "¦Ñ", "Rou", GH_ParamAccess.item, 28);
            pManager.AddNumberParameter("Beta", "¦Â", "Beta", GH_ParamAccess.item, (double)(8 / 3));
            pManager.AddNumberParameter("DeltaT", "¦¤t", "DeltaT", GH_ParamAccess.item, 0.01);
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
            double Sigma = 0.0;
            double Rou = 0.0;
            double Beta = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;


            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Sigma)) return;
            if (!DA.GetData(2, ref Rou)) return;
            if (!DA.GetData(3, ref Beta)) return;
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
            List<Point3d> LorenzOscillatorPoints = GenerateLorenzAttractor(StartPoint, Sigma, Rou, Beta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)LorenzOscillatorPoints;
            DA.SetDataList(0, __enum_points);

            var curve = Curve.CreateInterpolatedCurve(LorenzOscillatorPoints, 3);
            DA.SetData(1, curve);

        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> GenerateLorenzAttractor(Point3d StartPoint, double Sigma, double Rou, double Beta, double DeltaT, int Iterations)
        {
            point = StartPoint;
            newpoints = new List<Point3d>();

            double x = point.X;
            double y = point.Y;
            double z = point.Z;



            for (int i = 0; i < Iterations; i++)
            {

                newpoints.Add(point);
                double dx = Sigma * (y - x);
                double dy = x * (Rou - z) - y;
                double dz = x * y - Beta * z;

                x += dx * DeltaT;
                y += dy * DeltaT;
                z += dz * DeltaT;

                point = new Point3d(x, y, z);
            }


            return newpoints;

        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("8F39DA21-05AA-47F5-B22C-A91AFC4479F2");
    }
}