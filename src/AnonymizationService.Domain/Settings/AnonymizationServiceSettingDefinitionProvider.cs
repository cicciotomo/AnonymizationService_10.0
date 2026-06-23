using Volo.Abp.Settings;

namespace AnonymizationService.Settings;

public class AnonymizationServiceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AnonymizationServiceSettings.MySetting1));
    }
}
