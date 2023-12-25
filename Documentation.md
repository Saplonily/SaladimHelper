# Index

Hi! Thanks for using this small personal celeste helper. If you have encoutered any problems or found some bugs with it, always be free to ping me in the celescord (Saplonily#8059)!  
Also, here is the source code repository : [Github](https://github.com/Saplonily/SaladimHelper), you can raise issues and pull requests there too.  

English version at [Section English](#english).

## Chinese

### DirtBounceBlock

某位好友想出的三个新的 Core 模式之一的 Dirt 模式下的 BounceBlock  
需要冲刺撞击才会破碎.

- `None Core Mode`: 未使用参数.

### ReelCamera

一个很复杂的实体, 用于移动摄像机. **不太稳定**

- `Delay Sequence`: 延迟序列, 以逗号分隔, 个数需要为 节点数-1
- `Move Time Sequence`: 移动时间序列, 以逗号分隔, 个人需要为 节点数-1
- `Offset X`: 移动序列结束时要设置的摄像机偏移 X
- `Offset Y`: 移动序列结束时要设置的摄像机偏移 Y
- `Start Delay`: 移动序列开始时的延迟
- `Start Move Time`: 移动序列开始时的移动时间
- `Set Offset On Finished`: 移动序列结束后是否覆盖摄像机偏移为 `Offset X` 和 `Offset Y` 的值. 否则设置偏移为移动结束时的值
- `Squash Horizontal Area`: 移动过程中玩家水平方向掉出摄像机范围内时应该直接杀死玩家还是挤压玩家.

当玩家碰到该实体后摄像机会等待 `Start Delay` 秒然后在 `Start Move Time` 内移动到第一个节点的位置, 然后等待 `Start Sequence` 里的第一项的时间, 然后以 `Move Time Sequence` 第一项的时间移动到第二个节点, 以此类推.

### EnableFrostFreezeTrigger

给 FrostHelper 的 DreamBlock 加上冻结帧(或者你也可以在设置中打开 AlwaysEnableFrostFreeze).

### Key To Teleport Field

Trigger, 在此区域内玩家可以按下按键(默认为 A 键)来进行传送. **已过时**

- `AudioToPlay`: 传送时播放的音效
- `TargetRoomId`: 目标房间 ID.
- `Vector X`: 传送向量 X 分量
- `Vector Y`: 传送向量 Y 分量
- `Absolute`: 传送向量是否是绝对坐标. 如果是并且 `Cross Room` 为假, 则传送到相对于房间左上角的偏移处, 否则相对于玩家移动到偏移处.
- `Cross Room`: 是否是跨房间的, 是则播放特效并传送到对应房间. (为 true 并且未填写 `TargetRoomId` 会使游戏崩溃)
- `KillPlayerOnCollideWall`: 当玩家传送进墙里时是否直接杀死玩家.

### Filter

通用参数:

- `Easing`: 缓动方式
- `Effect Path`: 滤镜特效的路径
- `Index`: 序号
- `Strength`: 滤镜强度, 范围为 0 ~ 100, 可以超出范围但是行为未定义.
- `StrengthFrom`: 同 `Strength`, 开始滤镜强度
- `StrengthTo`: 同 `Strength`, 目标滤镜强度

目前支持的滤镜效果有:

- Blur
- Invert
- Gray
- MirrorX
- MirrorY

滤镜的作用顺序即为序号顺序, 序号越大越晚作用.  
当多个 Trigger 同时存在时, 修改的滤镜层目标为 Index 和 EffectPath 的组合, 两者任一不相同则会新创建滤镜层并修改.

#### FadeFilterTrigger

位置渐变滤镜 Trigger, 滤镜强度随玩家在 Trigger 内位置决定.

- `LeftToRight`: 决定是从左到右缓动还是从上到下

#### OneshotFilterTrigger

单发渐变滤镜 Trigger, 玩家进入 Trigger 后触发.

- `Duration`: 缓动时间
- `Once`: 是否只触发一次
- `StrengthFromCurrent`: 为 true 时忽略 `StrengthFrom` 参数, 并使用当前滤镜强度作为替代.

#### SpeedFadeFilterTrigger

速度渐变滤镜 Trigger, 玩家在内时根据速度的大小插值到滤镜强度.

- `Speed From`: 最低速度
- `Speed To`: 最高速度

#### SpeedOneshotFilterTrigger

速度单发渐变滤镜 Trigger, 玩家在内时达到一定速度阈值后开始缓动滤镜强度, 达不到时立刻回落.

- `Duration`: 缓动时间
- `Speed Threshold`: 速度阈值
- `StrengthFromCurrent`: 为 true 时忽略 `StrengthFrom` 参数, 并使用当前滤镜强度作为替代.

#### SpeedTwoWayShotFilterTrigger

速度双发渐变滤镜 Trigger, 同 [`SpeedOneshotFilterTrigger`](#speedoneshotfiltertrigger), 但是速度下降到一定数值时才会回落.

- `Speed Threshold`: 速度阈值
- `Speed Fall Threshold`: 回落的速度阈值

#### StaticFilterTrigger

静态滤镜强度更改 Trigger, 玩家进入时更改对应滤镜层强度.

### CustomAscendManager

从 草莓酱 2021 Collab 复制而来.

### DelayedFallingBlock

可调控时间信息的掉落块.

- `preDelay`: 玩家触摸后的固有等待, 默认 0.2s
- `playerWaitDelay`: 玩家触摸后的可通过离开掉落块而取消的等待, 默认 0.4s
- `noSfx`: 抖动时是否取消音效的播放
- `autoFall`: 是否自动掉落, 通常需要配合 `noSfx` 开启

## English

> Sorry for my poor English, will be thankful for correcting the following!

### DirtBounceBlock

BounceBlock in one of the three new core modes my friend came up with.  
Takes a dash to shatter.

- `None Core Mode`: unused.

### ReelCamera

A complex entity, used to move the camera. **Unstable**

- `Delay Sequence`: Delay sequence, separated by commas, needs to have (nodes count - 1) numbers.
- `Move Time Sequence`: Move time sequence, separated by commas, needs to have (nodes count - 1) numbers.
- `Offset X`: X of the Camera Offset to be set when finished.
- `Offset Y`: Y of the Camera Offset to be set when finished.
- `Start Delay`: The delay at the start of the move sequence.
- `Start Move Time`: The time will be taken to move at the start of the move sequence.
- `Set Offset On Finished`: Whether to set the Camera Offset with `Offset X` and `Offset Y` when finished. Otherwise set to the offset when finished.
- `Squash Horizontal Area`: Should the player be killed or squeezed if they fall out of the camera horizontally while moving.

After the player touched the entity, it waits for `Start Delay` seconds and then moves to the position of the first node in `Start Move Time` seconds, then waits for the time of the first item in the `Start Sequence`, then moves to the second node in the seconds of the first item in the `Move Time Sequence`, and so on.

### EnableFrostFreezeTrigger

Add freeze frames to FrostHelper's DreamBlocks (or you can turn on `AlwaysEnableFrostFreeze` in the mod settings).

### Key To Teleport Field

Trigger, player can teleport by pressing the key (A by default) while staying in the trigger. **Obsolete**

- `AudioToPlay`: The sound will play when teleporting.
- `TargetRoomId`: The target room ID.
- `Vector X`: vector X component
- `Vector Y`: vector Y component
- `Absolute`: Whether the teleportation vector is absolute. If it's true and `Cross Room` is false, then teleport to an offset relative to the top left corner of the room, otherwise move to an offset relative to the player.
- `Cross Room`: Whether it's cross-room, if it's true then the glitch effect will be played and player will be teleported to the target room. (`true` but not filling in `TargetRoomId` would crash the game)
- `KillPlayerOnCollideWall`: Whether or not the player is killed when teleporting into the wall.

### Filter

General parameters:

- `Easing`: Easing type
- `Effect Path`: The path of the filter
- `Index`: The index of the Filter
- `Strength`: The strength of the filter, range in 0 ~ 100, can be out of range but the behavior is unidentified.
- `StrengthFrom`: Same as `Strength`, the filter strength will be at the start.
- `StrengthTo`: Same as `Strength`, the filter strength will be at the end.

Currently supported filter type:

- Blur
- Invert
- Gray
- MirrorX
- MirrorY

The order which the filters will be in is the same as `Index`, the larger the `Index`, the later it will be applied.  
When multiple Triggers exist at the same time, the modified filter layer will be distinguished by the `Index` and `EffectPath`. If either of the two is different, a new filter layer is created and modified.

#### FadeFilterTrigger

Position gradient filter trigger, the strength of the filter depends on the player's position in the Trigger.

- `LeftToRight`: Decide whether to ease from left to right or top to bottom

#### OneshotFilterTrigger

single-shot gradient filter trigger, triggered when the player enters the trigger.

- `Duration`: Easing time
- `Once`: whether it will be triggered only once
- `StrengthFromCurrent`: Ignores the `StrengthFrom` parameter when `true`, and uses the current filter strength instead.

#### SpeedFadeFilterTrigger

Speed gradient filter trigger, the strength of the filter is based on the speed of the player when the player is inside.

- `Speed From`: The minimum speed.
- `Speed To`: The maximum speed.

#### SpeedOneshotFilterTrigger

Speed single-shot gradient filter trigger, the player starts to ease the filter strength when the player reaches a certain speed threshold, and immediately drops back when it is not reached.

- `Duration`: Easing time
- `Speed Threshold`: The speed threshold
- `StrengthFromCurrent`: Ignores the `StrengthFrom` parameter when true, and uses the current filter strength as an alternative.

#### SpeedTwoWayShotFilterTrigger

The speed dual-shot Filter Trigger, the same as [`SpeedOneshotFilterTrigger`](#speedoneshotfiltertrigger), but fallback when the speed reaches the lower threshold.

- `Speed Threshold`: The speed threshold
- `Speed Fall Threshold`: The speed threshold when the strength will fallback.

#### StaticFilterTrigger

Static filter trigger, change the strength when the player enters.

### CustomAscendManager

Copied from Strawberry Jam 2021 Collab.

### DelayedFallingBlock

A falling block with adjustable time parameters.

- `preDelay`: Fixed wait time after player touched, default to 0.2s.
- `playerWaitDelay`: Wait time after player touched that can be canceled by leaving the falling block, default to 0.4s.
- `noSfx`: Whether to disable sound effects during shaking. Usually true with `autoFall` set to true.
- `autoFall`: Whether to fall automatically.