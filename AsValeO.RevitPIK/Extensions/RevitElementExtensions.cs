using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RoomFinishes.Extensions
{
    public static class RevitElementExtensions
    {
        public static string GetParam(this Element element, string paramName) =>
            element.GetParameters(paramName).FirstOrDefault() != null
                ? element.GetParameters(paramName).FirstOrDefault()?.AsString()
                : null;

        private static int GetAparmentNumber(this Element element) =>
            int.TryParse(GetParam(element, "ROM_Зона").NthFromSplit(" ", 2), out var o) ? o : -1;

        public static IEnumerable<Element> SelectRepaintRequired(this List<Element> elements) =>
            elements.Select(e => new {element = e, number = GetAparmentNumber(e)})
                .Where(t => elements.Select(GetAparmentNumber).Any(y => y == t.number + 1)).Select(t => t.element);
    }
}