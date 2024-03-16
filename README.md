# SaladimHelper

## 简介
个人杂乱的蔚蓝 mod, 然后没啥好说的了(  
统一的个人蔚蓝 mod 更新贴在[这里](https://celeste.centralteam.cn/d/93-sapqi-miao-modgeng-xin-tie)

## 构建

本项目采用 [sap 的蔚蓝 mod 教程](https://celestemod.saplonily.link/begin/BasicEnv/) 中的 [模板](https://github.com/Saplonily/celeste-mod-template-sdkstyled).
如果你需要自行构建项目的话请到 `Common.props` 文件中将 `CelesteRootPath` 更改为你的蔚蓝游戏路径.  

mod 的 dll/pdb 资源都将由 msbuild 复制. 默认会将 `ModFolder` 目录复制到到 `蔚蓝游戏路径/Mods/<ModName>_copy`, 所以对于资源文件建议放到 `ModFolder` 文件夹内,
并且**不要**将项目文件放置到 Mods 文件夹里.

# SaladimHelper

## Introduction
A personal random mod for Celeste.

## Building
This project uses the [Saladim.CelesteModTemplate](https://github.com/Saplonily/celeste-mod-template-sdkstyled).
If you want to build this project on your own, please change the `CelesteRootPath` in `Common.props` to your Celeste game path.

dll/pdb resources of the mod will be copied by msbuild. The `ModFolder` directory will be copied to `<Celeste game path>/Mods/<ModName>_copy` by default, so it is recommended to put resource files in the `ModFolder` directory.
And by the way, **do not** put project files in the Celeste `Mods` directory though some of mods do.