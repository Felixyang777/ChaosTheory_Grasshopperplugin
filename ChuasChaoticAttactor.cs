using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChaosTheory
{
    public class ChuasChaoticAttactor : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ChuasChaoticAttactor()
          : base("ChuasChaoticAttactor", "LOsc",
            "Construct a ChuasChaoticAttactor",
            "ChaosTheory", "Oscillator")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.

            pManager.AddPointParameter("StartPoint", "P", "StartPoint", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sigma", "¦Ò", "Sigma", GH_ParamAccess.item, 10.0);
            pManager.AddNumberParameter("Rou", "¦Ñ", "Rou", GH_ParamAccess.item);
            pManager.AddNumberParameter("Beta", "¦Â", "Beta", GH_ParamAccess.item, (double)8 / 3);
            pManager.AddNumberParameter("DeltaT", "¦¤t", "DeltaT", GH_ParamAccess.item, 0.01);
            pManager.AddIntegerParameter("Iterations", "I", "Number of  iterations", GH_ParamAccess.item, 100);

            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.

            pManager.AddPointParameter("Points", "P", "LorenzOscillator", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "C", "LorenzOscillator", GH_ParamAccess.item);
            
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.

            Point3d StartPoint = Point3d.Origin;
            double Sigma = 0.0;
            double Rou = 0.0;
            double Beta = 0.0;
            double DeltaT = 0.0;
            int Iterations = 100;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref StartPoint)) return;
            if (!DA.GetData(1, ref Sigma)) return;
            if (!DA.GetData(2, ref Rou)) return;
            if (!DA.GetData(3, ref Beta)) return;
            if (!DA.GetData(4, ref DeltaT)) return;
            if (!DA.GetData(5, ref Iterations)) return;
            // We should now validate the data and warn the user if invalid data is supplied.
            List<Point3d> LorenzOscillatorPoints = CreatLorenzOscillator(StartPoint, Sigma, Rou, Beta, DeltaT, Iterations);
            IEnumerable __enum_points = (IEnumerable)LorenzOscillatorPoints;
            DA.SetDataList(0, __enum_points);
            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 

            // Finally assign the spiral to the output parameter.
        }

        List<Point3d> newpoints;
        Point3d point;
        List<Point3d> CreatLorenzOscillator(Point3d StartPoint, double Sigma, double Rou, double Beta, double DeltaT,int Iterations)
        {
            point = StartPoint;

            double x0=point.X;
            double y0=point.Y;
            double z0=point.Z;

            newpoints = new List<Point3d>();

            for (int i = 0; i < Iterations; i++)
            {
                point = new Point3d(x0, y0, z0);
                newpoints.Add(point);
                double deltax = Sigma * (y0 - x0);
                double deltay = x0 * (Rou - z0) - y0;
                double deltaz = x0 * y0 - Beta * z0;
                x0 += deltax * DeltaT;
                y0 += deltay * DeltaT;
                z0 += deltaz * DeltaT;
            }


            return newpoints;

        }
       
        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("BF88E838-9858-4175-8253-D276354E1A13");
    }
}