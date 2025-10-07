using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoomManagement.RevitCommands.Commands
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class RoomToFilledRegion : IExternalCommand
    {
        public static UIDocument uidoc { get; set; }
        public static Document doc { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ISelectionFilter RoomFilter = new RoomSelectionFilter();

            Reference sel = uidoc.Selection.PickObject(ObjectType.Element, RoomFilter);
            Element elem = doc.GetElement(sel);
            Room selectedRoom = elem as Room;
            GetInfo_BoundarySegment(selectedRoom);


            return Result.Succeeded;
        }
        public void GetInfo_BoundarySegment(Room room)
        {
            IList<IList<Autodesk.Revit.DB.BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            if (null != segments)  //the room may not be bound
            {
                string message = "BoundarySegment";
                IList<Curve> curves = new List<Curve>();
                foreach (IList<Autodesk.Revit.DB.BoundarySegment> segmentList in segments)
                {
                    foreach (Autodesk.Revit.DB.BoundarySegment boundarySegment in segmentList)
                    {
                        curves.Add(boundarySegment.GetCurve());

                        // Get curve start point
                        message += "\nCurve start point: (" + boundarySegment.GetCurve().GetEndPoint(0).X + ","
                                       + boundarySegment.GetCurve().GetEndPoint(0).Y + "," +
                                      boundarySegment.GetCurve().GetEndPoint(0).Z + ")";
                        // Get curve end point
                        message += ";\nCurve end point: (" + boundarySegment.GetCurve().GetEndPoint(1).X + ","
                             + boundarySegment.GetCurve().GetEndPoint(1).Y + "," +
                             boundarySegment.GetCurve().GetEndPoint(1).Z + ")";
                        // Get document path name
                        message += ";\nDocument path name: " + room.Document.PathName;
                        // Get boundary segment element name
                        if (boundarySegment.ElementId != ElementId.InvalidElementId)
                        {
                            message += ";\nElement name: " + room.Document.GetElement(boundarySegment.ElementId).Name;
                        }
                    }
                }




                FilledRegionType filledRegionType = new FilteredElementCollector(doc)
        .OfClass(typeof(FilledRegionType))
        .Cast<FilledRegionType>()
        .FirstOrDefault(frt => frt.Name == "Solid Black"); // Or another desired name
                CurveLoop curveLoop = CurveLoop.Create(curves);
                using (Transaction t = new Transaction(doc, "Crop View"))
                {
                    t.Start();
                    View activeView = doc.ActiveView;

                    FilledRegion filledRegion = FilledRegion.Create(doc, filledRegionType.Id, activeView.Id, new List<CurveLoop>() { curveLoop });

                    t.Commit();
                }
                TaskDialog.Show("Revit", message);
            }
        }

    }
}
