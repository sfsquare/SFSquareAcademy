using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SFSAcademy.Helpers
{
    public abstract class NoResubmitAbstract
    {
        public Guid PreventResubmit { get; set; }

        public void SetPreventResubmit(Guid prs)
        {
            PreventResubmit = prs;
        }
    }

    public static class ControllerExtentions
    {
        [NonAction]
        public static bool IsResubmit(this System.Web.Mvc.ControllerBase controller, NoResubmitAbstract vModel)
        {
            return (Guid)controller.TempData["PreventResubmit"] != vModel.PreventResubmit;
        }

        [NonAction]
        public static void PreventResubmit(this System.Web.Mvc.ControllerBase controller, params NoResubmitAbstract[] vModels)
        {
            var preventResubmitGuid = Guid.NewGuid();
            controller.TempData["PreventResubmit"] = preventResubmitGuid;
            foreach (var vm in vModels)
            {
                vm.SetPreventResubmit(preventResubmitGuid);
            }
        }
    }

}
