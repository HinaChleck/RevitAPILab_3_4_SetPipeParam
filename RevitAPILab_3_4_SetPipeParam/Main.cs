using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPILab_3_4_SetPipeParam
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        //Устанавливает в существующий параметр экземпляра проекта для труб "Наименование" в формате «Труба НАРУЖНЫЙ_ДИАМЕТР / ВНУТРЕННИЙ_ДИАМЕТР», где НАРУЖНЫЙ_ДИАМЕТР и ВНУТРЕННИЙ_ДИАМЕТР соответствующие диаметры трубы в миллиметрах.
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Application application = uiapp.Application;

            List<Pipe> pipes = new FilteredElementCollector(doc)
            .OfClass(typeof(Pipe))
            //.WhereElementIsNotElementType()// не требуется при фильтрации по классу (требуется при фильтрации по категории)
            .Cast<Pipe>()
            .ToList();

            foreach (var pipe in pipes)
            {
                string innerDiam = UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble(), DisplayUnitType.DUT_MILLIMETERS).ToString();
                string outerDiam = UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble(), DisplayUnitType.DUT_MILLIMETERS).ToString();

                using (Transaction ts = new Transaction(doc, "Set parameter"))
                {
                    ts.Start();

                    Parameter NameParameter = pipe.LookupParameter("Наименование");
                    NameParameter.Set($"Труба {innerDiam}/{outerDiam}");

                    ts.Commit();
                }
            }

            return Result.Succeeded;
        }

    }
}
