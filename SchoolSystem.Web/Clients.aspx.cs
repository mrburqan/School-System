using Microsoft.Practices.Unity;
using SchoolSystem.Application.Models.Request;
using SchoolSystem.Application.Services.IServices;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Unity.WebForms;

namespace SchoolSystem.Web
{
    public partial class Clients : System.Web.UI.Page
    {
        private IGenderService _genderService;
        private ITypeService _typeService;

        protected void Page_Init(object sender, EventArgs e)
        {
            var container = HttpContext.Current.Application.GetContainer() as IUnityContainer;

            _genderService = container.Resolve<IGenderService>();
            _typeService = container.Resolve<ITypeService>();
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await LoadGender();
                await LoadTypes();
            }
        }

        private async Task LoadGender()
        {
            var request = new PageRequest { IsAscending = true };

            var genders = await _genderService.GetPageAsync(request);

            ddlGender.DataSource = genders;
            ddlGender.DataTextField = "Name";
            ddlGender.DataValueField = "Id";
            ddlGender.DataBind();

            ddlGender.Items.Insert(0, new ListItem("All", ""));
        }

        private async Task LoadTypes()
        {
            var request = new PageRequest { IsAscending = true };

            var types = await _typeService.GetPageAsync(request);

            ddlType.DataSource = types;
            ddlType.DataTextField = "Name";
            ddlType.DataValueField = "Id";
            ddlType.DataBind();

            ddlType.Items.Insert(0, new ListItem("All", ""));
        }
    }
}