//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PDDLTools.Projects
{
    
    
    internal partial class ConfigurationGeneralBrowseObject
    {
        
        /// <summary>The name of the schema to look for at runtime to fulfill property access.</summary>
        internal const string SchemaName = "ConfigurationGeneralBrowseObject";
        
        /// <summary>The ItemType given in the Rule.DataSource property.  May not apply to every Property's individual DataSource.</summary>
        internal const string PrimaryDataSourceItemType = null;
        
        /// <summary>The Label given in the Rule.DataSource property.  May not apply to every Property's individual DataSource.</summary>
        internal const string PrimaryDataSourceLabel = "Configuration";
        
        /// <summary> (The "FileName" property).</summary>
        internal const string FileNameProperty = "FileName";
        
        /// <summary> (The "FullPath" property).</summary>
        internal const string FullPathProperty = "FullPath";
        
        /// <summary>Backing field for the <see cref='Microsoft.Build.Framework.XamlTypes.Rule'/> property.</summary>
        private Microsoft.VisualStudio.ProjectSystem.Properties.IRule rule;
        
        /// <summary>Initializes a new instance of the ConfigurationGeneralBrowseObject class.</summary>
        internal ConfigurationGeneralBrowseObject(Microsoft.VisualStudio.ProjectSystem.Properties.IRule rule)
        {
            this.rule = rule;
        }
        
        /// <summary>Initializes a new instance of the ConfigurationGeneralBrowseObject class.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="configuredProject", Justification="This is only used if the user opts the rule in for fallback rule creation or they" +
            " provide a missingruleerror or missingpropertyerror")]
        internal ConfigurationGeneralBrowseObject(Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject, System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog> catalogs, string context, string file, string itemType, string itemName) : 
                this(GetRule(System.Collections.Immutable.ImmutableDictionary.GetValueOrDefault(catalogs, context), file, itemType, itemName))
        {
        }
        
        /// <summary>Initializes a new instance of the ConfigurationGeneralBrowseObject class.</summary>
        internal ConfigurationGeneralBrowseObject(Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject, System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog> catalogs, string context, Microsoft.VisualStudio.ProjectSystem.Properties.IProjectPropertiesContext propertyContext) : 
                this(configuredProject, catalogs, context, GetContextFile(propertyContext), propertyContext.ItemType, propertyContext.ItemName)
        {
        }
        
        /// <summary>Initializes a new instance of the ConfigurationGeneralBrowseObject class that assumes a project context (neither property sheet nor items).</summary>
        internal ConfigurationGeneralBrowseObject(Microsoft.VisualStudio.ProjectSystem.ConfiguredProject configuredProject, System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog> catalogs) : 
                this(configuredProject, catalogs, "Project", null, null, null)
        {
        }
        
        /// <summary>Gets the IRule used to get and set properties.</summary>
        public Microsoft.VisualStudio.ProjectSystem.Properties.IRule Rule
        {
            get
            {
                return this.rule;
            }
        }
        
        /// <summary>FileName</summary>
        internal Microsoft.VisualStudio.ProjectSystem.Properties.IEvaluatedProperty FileName
        {
            get
            {
                Microsoft.VisualStudio.ProjectSystem.Properties.IRule localRule = this.rule;
                if ((localRule == null))
                {
                    return null;
                }
                Microsoft.VisualStudio.ProjectSystem.Properties.IEvaluatedProperty property = ((Microsoft.VisualStudio.ProjectSystem.Properties.IEvaluatedProperty)(localRule.GetProperty(FileNameProperty)));
                return property;
            }
        }
        
        /// <summary>FullPath</summary>
        internal Microsoft.VisualStudio.ProjectSystem.Properties.IEvaluatedProperty FullPath
        {
            get
            {
                Microsoft.VisualStudio.ProjectSystem.Properties.IRule localRule = this.rule;
                if ((localRule == null))
                {
                    return null;
                }
                Microsoft.VisualStudio.ProjectSystem.Properties.IEvaluatedProperty property = ((Microsoft.VisualStudio.ProjectSystem.Properties.IEvaluatedProperty)(localRule.GetProperty(FullPathProperty)));
                return property;
            }
        }
        
        private static Microsoft.VisualStudio.ProjectSystem.Properties.IRule GetRule(Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog catalog, string file, string itemType, string itemName)
        {
            if ((catalog == null))
            {
                return null;
            }
            return catalog.BindToContext(SchemaName, file, itemType, itemName);
        }
        
        private static string GetContextFile(Microsoft.VisualStudio.ProjectSystem.Properties.IProjectPropertiesContext propertiesContext)
        {
            if ((propertiesContext.IsProjectFile == true))
            {
                return null;
            }
            else
            {
                return propertiesContext.File;
            }
        }
    }
    
    internal partial class ProjectProperties
    {
        
        private static System.Func<System.Threading.Tasks.Task<System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog>>, object, ConfigurationGeneralBrowseObject> CreateConfigurationGeneralBrowseObjectPropertiesDelegate = new System.Func<System.Threading.Tasks.Task<System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog>>, object, ConfigurationGeneralBrowseObject>(CreateConfigurationGeneralBrowseObjectProperties);
        
        private static ConfigurationGeneralBrowseObject CreateConfigurationGeneralBrowseObjectProperties(System.Threading.Tasks.Task<System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog>> namedCatalogs, object state)
        {
            ProjectProperties that = ((ProjectProperties)(state));
            return new ConfigurationGeneralBrowseObject(that.ConfiguredProject, namedCatalogs.Result, "Project", that.File, that.ItemType, that.ItemName);
        }
        
        /// <summary>Gets the strongly-typed property accessor used to get and set General properties.</summary>
        internal System.Threading.Tasks.Task<ConfigurationGeneralBrowseObject> GetConfigurationGeneralBrowseObjectPropertiesAsync()
        {
            System.Threading.Tasks.Task<System.Collections.Immutable.IImmutableDictionary<string, Microsoft.VisualStudio.ProjectSystem.Properties.IPropertyPagesCatalog>> namedCatalogsTask = this.GetNamedCatalogsAsync();
            return namedCatalogsTask.ContinueWith(CreateConfigurationGeneralBrowseObjectPropertiesDelegate, this, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously, System.Threading.Tasks.TaskScheduler.Default);
        }
        
        /// <summary>Gets the strongly-typed property accessor used to get value from the current project snapshot General properties.</summary>
        internal bool TryGetCurrentConfigurationGeneralBrowseObjectPropertiesSnapshot(out ConfigurationGeneralBrowseObject snapshot, [System.Runtime.InteropServices.OptionalAttribute()] [System.Runtime.InteropServices.DefaultParameterValueAttribute(true)] bool requiredToMatchProjectVersion)
        {
            snapshot = null;
            Microsoft.VisualStudio.ProjectSystem.IProjectVersionedValue<Microsoft.VisualStudio.ProjectSystem.Properties.IProjectCatalogSnapshot> catalogSnapshot;
            if (this.TryGetCurrentCatalogSnapshot(out catalogSnapshot))
            {
                if (requiredToMatchProjectVersion)
                {
                    if ((this.ConfiguredProject.ProjectVersion.CompareTo(catalogSnapshot.DataSourceVersions[Microsoft.VisualStudio.ProjectSystem.ProjectDataSources.ConfiguredProjectVersion]) != 0))
                    {
                        return false;
                    }
                }
                Microsoft.VisualStudio.ProjectSystem.Properties.IRule rule = this.GetSnapshotRule(catalogSnapshot.Value, "Project", ConfigurationGeneralBrowseObject.SchemaName);
                if ((rule != null))
                {
                    snapshot = new ConfigurationGeneralBrowseObject(rule);
                    return true;
                }
            }
            return false;
        }
    }
}
