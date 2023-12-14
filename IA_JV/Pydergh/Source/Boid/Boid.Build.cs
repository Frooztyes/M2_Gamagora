// Copyright Epic Games, Inc. All Rights Reserved.

using System.IO;
using UnrealBuildTool;

public class Boid : ModuleRules
{
	public Boid(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PrivatePCHHeaderFile = "BoidsPrecompiled.h";

        PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore", "EnhancedInput", "Niagara" });


        MinFilesUsingPrecompiledHeaderOverride = 1;

        bUseUnity = false;
    }
}
