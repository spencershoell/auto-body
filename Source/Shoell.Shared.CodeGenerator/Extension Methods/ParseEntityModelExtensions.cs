using System.Xml;
using Shoell.Autobody.CodeGenerator.Models;

namespace Shoell.Autobody.CodeGenerator
{
    public static class ParseEntityModelExtensions
    {
        public static EntityType? ParseEntityModel(this XmlNode node, string ns)
        {
            if (node.Name == "EntityType")
            {
                var model = new EntityType();
                if (node.Attributes != null && node.Attributes["Name"] != null)
                {
                    model.Name = node.Attributes["Name"]!.Value;
                }

                foreach (XmlNode key in node.ChildNodes)
                {
                    if (key.Name == "Key")
                    {
                        foreach (XmlNode propertyRef in key.ChildNodes)
                        {
                            if (propertyRef.Name == "PropertyRef")
                            {
                                if (propertyRef.Attributes != null && propertyRef.Attributes["Name"] != null)
                                {
                                    model.Keys.Add(new Key { Name = propertyRef.Attributes["Name"]!.Value });
                                }
                            }
                        }
                    }
                }

                foreach (XmlNode property in node.ChildNodes)
                {
                    if (property.Name == "Property" && property.Attributes != null)
                    {
                        var propertyModel = new Property();

                        if (property.Attributes["Name"] != null)
                            propertyModel.Name = property.Attributes["Name"]!.Value;

                        if (property.Attributes["Type"] != null)
                            propertyModel.Type = property.Attributes["Type"]!.Value;

                        if (property.Attributes["Nullable"] != null)
                            if (bool.TryParse(property.Attributes["Nullable"]!.Value, out var nullable))
                                propertyModel.Nullable = nullable;

                        //if (property.Attributes["Scale"] != null)
                        //propertyModel.Scale = property.Attributes["Scale"]!.Value;

                        //if (property.Attributes["Precision"] != null)
                        //propertyModel.Precision = property.Attributes["Precision"]!.Value;

                        model.Properties.Add(propertyModel);
                    }
                }

                foreach (XmlNode navigationProperty in node.ChildNodes)
                {
                    if (navigationProperty.Name == "NavigationProperty" && navigationProperty.Attributes != null)
                    {
                        var navPropertyModel = new NavigationProperty();

                        if (navigationProperty.Attributes["Name"] != null)
                            navPropertyModel.Name = navigationProperty.Attributes["Name"]!.Value;

                        if (navigationProperty.Attributes["Type"] != null)
                            navPropertyModel.Type = navigationProperty.Attributes["Type"]!.Value;

                        if (navigationProperty.Attributes["Partner"] != null)
                            navPropertyModel.Partner = navigationProperty.Attributes["Partner"]!.Value;

                        foreach (XmlNode referentialConstraint in navigationProperty.ChildNodes)
                        {
                            if (referentialConstraint.Name == "ReferentialConstraint" && referentialConstraint.Attributes != null)
                            {
                                //var rcModel = new ReferentialConstraint();

                                //if (referentialConstraint.Attributes["Property"] != null)
                                //rcModel.ForeignKey = referentialConstraint.Attributes["Property"]!.Value;

                                //if (referentialConstraint.Attributes["ReferencedProperty"] != null)
                                //rcModel.PrincipalKey = referentialConstraint.Attributes["ReferencedProperty"]!.Value;
                                //navPropertyModel.ReferentialConstraints.Add(rcModel);
                            }
                        }

                        model.NavigationProperties.Add(navPropertyModel);
                    }
                }
                return model;
            }
            return null;
        }
    }
}
