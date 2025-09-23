using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

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


            throw new NotImplementedException();
        }
    }
}
