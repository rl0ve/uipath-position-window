using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using UiPathTeam.Positioner.Activities.Design.Properties;

namespace UiPathTeam.Positioner.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute =  new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(PositionWindow), categoryAttribute);
            builder.AddCustomAttributes(typeof(PositionWindow), new DesignerAttribute(typeof(PositionWindowDesigner)));
            builder.AddCustomAttributes(typeof(PositionWindow), new HelpKeywordAttribute("https://go.uipath.com"));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
