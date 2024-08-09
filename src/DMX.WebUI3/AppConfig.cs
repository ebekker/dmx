// DMX.
// Copyright (C) Eugene Bekker.

namespace DMX.WebUI3;

public class AppConfig
{
    public const string DefaultSectionName = nameof(DMX);

    public bool ApplyMigrations { get; set; }
    public bool SkipMigrationsCheck { get; set; }
    public bool PopulateTestModel { get; set; }

    public bool SkipProgramRun { get; set; }
}
