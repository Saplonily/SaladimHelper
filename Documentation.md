# Index

Hi! Thanks for using this small personal celeste helper. If you have encoutered any problems or found some bugs with it, always feel free to ping me in the celescord (Saplonily#8059)!  
Also, here is the source code repository : [Github](https://github.com/Saplonily/SaladimHelper), you can raise any issues and pull requests there too.  

English version is at [Section English](#english).

## Chinese

### DirtBounceBlock

某位好友想出的三个新的 Core 模式之一的 Dirt 模式下的 BounceBlock  
需要冲刺撞击才会破碎.

- `textureBasePath`: 自定义贴图目录路径, 参考 "SaladimHelper/entities/moreBounceBlock"
- `imageSprite`: 自定义中心图片, 参考 Sprites.xml 中的 "sal_bumpBlockCenterDirt" (Loenn 显示的贴图不会被影响, 反而它会使用 textureBasePath 下的 "rock_center00")
- `revertFallDir`: 是否反重力掉落

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

给 FrostHelper 的 DreamBlock 加上冻结帧.

### CustomAscendManager

从 草莓酱 2021 Collab 复制而来.

### DelayedFallingBlock

可调控时间信息的掉落块.

- `preDelay`: 玩家触摸后的固有等待, 默认 0.2s
- `playerWaitDelay`: 玩家触摸后的可通过离开掉落块而取消的等待, 默认 0.4s
- `noSfx`: 抖动时是否取消音效的播放
- `autoFall`: 是否自动掉落, 通常需要配合 `noSfx` 开启

### FlagRefill

使用后获得 Flag 的水晶.

- `flag`: flag
- `useOnlyNoFlag`: 仅在没有flag时才能使用
- `removeFlagDelay`: 当冲刺开始后应该多久后移除 Flag

### ExplodeFocusArea

爆炸聚焦区域, 它会尝试将所在区域内的爆炸方向固定为四向, 八向等等.

- `focusType`: 固定类型
- `rotation`: 固定方向的旋转

### PositionBlock

位置方块, 它的位置会根据玩家相对于其的位置而改变. 它的节点通常应该在 x 或 y 坐标上与本体一致.

- `speed`: 改变的速度
- `easing`: 位置变更所使用的缓动
- `range`: 生效范围, 0 为仅在方块本身位置, 其他表示向周围拓展几个像素

## English

> Sorry for my poor English, will be thankful for correcting the following!

### DirtBounceBlock

BounceBlock in one of the three new core modes my friend came up with.  
Takes a dash to shatter.

- `textureBasePath`: Custom texture base path, will be a path like "SaladimHelper/entities/moreBounceBlock".
- `imageSprite`: Custom center image, will be like "sal_bumpBlockCenterDirt" in Sprites.xml (This will not affect texture shown in loenn, it'll search for "rock_center00" under textureBasePath instead).
- `revertFallDir`: Whether to revert the fall direction (so it falls up).

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

Add freeze frames to FrostHelper's DreamBlocks.

### CustomAscendManager

Copied from Strawberry Jam 2021 Collab.

### DelayedFallingBlock

A falling block with adjustable time parameters.

- `preDelay`: Fixed wait time after player touched, default to 0.2s.
- `playerWaitDelay`: Wait time after player touched that can be canceled by leaving the falling block, default to 0.4s.
- `noSfx`: Whether to disable sound effects during shaking. Usually true with `autoFall` set to true.
- `autoFall`: Whether to fall automatically.

### FlagRefill

A refill that set flag on using.

- `flag`: flag.
- `useOnlyNoFlag`: Whether this refill can be used with no flag set.
- `removeFlagDelay`: Delay to remove flag after dash start.

### ExplodeFocusArea

Explode Focus Area, it will try to fix the direction of the explosion to four directions, eight directions, etc.

- `focusType`: focus type.
- `rotation`: rotation of the fix directions.

### PositionBlock

Position Block, its position will change according to the player's position relative to it. The nodes should usually be at the same x or y coordinate as the block itself.

- `speed`: the speed it changes its position.
- `easing`: easing of the position change.
- `range`: the range of effect, 0 for only the block itself, other for expanding around the block by pixels.