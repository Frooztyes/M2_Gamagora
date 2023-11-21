// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class Boid : ModuleRules
{
	public Boid(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PrivatePCHHeaderFile = "BoidsPrecompiled.h";

        PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore", "EnhancedInput" });

        MinFilesUsingPrecompiledHeaderOverride = 1;

        bUseUnity = false;
    }
}
