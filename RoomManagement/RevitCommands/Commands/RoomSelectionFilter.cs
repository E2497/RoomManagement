using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace RoomManagement.RevitCommands.Commands
{
    internal class RoomSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Name == "Rooms")
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;

        }
    }
}