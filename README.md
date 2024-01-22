# SaladimHelper

## 简介
个人杂乱的蔚蓝 mod, 然后没啥好说的了(  
统一的个人蔚蓝 mod 更新贴在[这里](https://celeste.centralteam.cn/d/93-sapqi-miao-modgeng-xin-tie)

## 构建

本项目采用[sap 的蔚蓝 mod 教程](https://celestemod.saplonily.link/begin/BasicEnv/)中的[模板](https://github.com/Saplonily/celeste-mod-template-sdkstyled).
如果你需要自行构建项目的话请到 `Common.props` 文件中将 `CelesteRootPath` 更改为你的蔚蓝游戏路径.  

mod 的 dll/pdb 资源都将由 msbuild 复制. 默认会为 `ModFolder` 目录建立软链接到 `蔚蓝游戏路径/Mods/<ModName>_link`, 所以对于资源文件建议放到 `ModFolder` 文件夹内.
