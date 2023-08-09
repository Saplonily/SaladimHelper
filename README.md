# SaladimHelper

## 简介
个人杂乱的蔚蓝mod, 然后没啥好说的了(  
统一的个人蔚蓝mod更新贴在[这里](https://celeste.centralteam.cn/d/93-sapqi-miao-modgeng-xin-tie)

## 构建

本项目采用[sap的蔚蓝mod教程](https://celestemod.saplonily.link/begin/BasicEnv/)中的[模板](https://github.com/Saplonily/celeste-mod-template-sdkstyled).
如果你需要自行构建项目的话请到 `Common.props` 文件中将 `CelesteRootPath` 更改为你的蔚蓝游戏路径.  

如果你还使用了 core 版本的 everest, 那么你需要在路径后面再加上目录 `legacyRef` 并在 everest 的 `mod 选项` 中选择 `setup legacyRef` 项.  

所有 mod 的 dll/pdb/resource 资源都将由 msbuild 复制, 默认复制行为为 `ModFolder` 到 `蔚蓝游戏路径/Mods/<ModName>/`, 所以对于资源文件建议放到 `ModFolder` 文件夹内.