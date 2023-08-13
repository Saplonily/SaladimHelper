namespace Celeste.Mod.SaladimHelper.Entities;

// 我知道这坨代码写的很烂但是你先别急整体都写完了再整理

public class MaybeAShopUI : Entity
{
    public const float CenterOffsetX = 182f;
    public const float CenterOffsetY = 190f;
    public readonly static Color BgColor = Calc.HexToColor("040345");

    private int lineMax;
    private int itemsCount;
    private int selectedIndex = 0;
    private List<(Image img, MTexture tex, MTexture texl)> items;

    private Action<MaybeAShopUI> ended;
    private Action<MaybeAShopUI, int> bought;

    private string[] texs;
    private float[] costs;

    private MTexture t;
    private MTexture b;
    private MTexture l;
    private MTexture r;
    private Vector2 itemSize;
    private (int columns, int rows) itemRowColumn;

    public MaybeAShopUI(Action<MaybeAShopUI, int> bought, Action<MaybeAShopUI> ended, string[] texs, float[] costs, int lineMax)
    {
        this.ended = ended;
        // don't ask me why i am doing these
        this.ended += _ =>
        {
            Input.Dash.ConsumePress(); Input.CrouchDash.ConsumePress();
            Input.Grab.ConsumePress(); Input.Jump.ConsumeBuffer();
        };
        this.bought = bought;
        this.costs = costs;
        this.texs = texs;
        this.lineMax = lineMax;
        Tag |= Tags.HUD | Tags.PauseUpdate;
        items = new();
        itemsCount = costs.Length;

        // 9patch
        t = GFX.Gui["SaladimHelper/MaybeAShop/t"];
        b = GFX.Gui["SaladimHelper/MaybeAShop/b"];
        l = GFX.Gui["SaladimHelper/MaybeAShop/l"];
        r = GFX.Gui["SaladimHelper/MaybeAShop/r"];
        itemSize = new(t.Width, t.Height);

        Vector2 topLeftFramePos = new();
        Vector2 bottomRightFramePos = new();
        int totalColumns = Math.Min(lineMax, itemsCount);
        int totalRows = (int)Math.Ceiling((double)itemsCount / lineMax);
        float totalWidth = totalColumns * itemSize.X;
        float totalHeight = totalRows * itemSize.Y;
        itemRowColumn = new(totalColumns, totalRows);
        int curRow = 0;
        int curColumn = 0;
        for (int i = 0; i < itemsCount; i++)
        {
            var tex = ChooseTexture(i, false);
            var texl = ChooseTexture(i, true);
            // 物品框贴图
            Image frameImg = new(tex);
            items.Add((frameImg, tex, texl));
            Vector2 screenCenter = new(Engine.Width / 2, Engine.Height / 2);
            Vector2 framePos = screenCenter + new Vector2(-totalWidth / 2 + curColumn * itemSize.X, -totalHeight / 2 + curRow * itemSize.Y);
            frameImg.Position = framePos;
            Add(frameImg);
            if (i == 0)
                topLeftFramePos = framePos;
            else if (curRow == totalRows - 1)
                bottomRightFramePos.Y = framePos.Y;
            else if (curColumn == totalColumns - 1)
                bottomRightFramePos.X = framePos.X;

            // 物品贴图
            var itemImg = new Image(GFX.Gui[texs[i]]);
            itemImg.Position = framePos + new Vector2(CenterOffsetX, CenterOffsetY);
            itemImg.CenterOrigin();
            Add(itemImg);

            curColumn++;
            if (curColumn >= lineMax)
            {
                curRow++;
                curColumn = 0;
            }
        }

        // bound textures
        MTexture tl = GFX.Gui["SaladimHelper/MaybeAShop/tl"];
        MTexture br = GFX.Gui["SaladimHelper/MaybeAShop/br"];
        MTexture tr = GFX.Gui["SaladimHelper/MaybeAShop/tr"];
        MTexture bl = GFX.Gui["SaladimHelper/MaybeAShop/bl"];
        Add(new Image(tl) { Position = topLeftFramePos });
        Add(new Image(br) { Position = bottomRightFramePos });
        Add(new Image(tr) { Position = new(bottomRightFramePos.X, topLeftFramePos.Y) });
        Add(new Image(bl) { Position = new(topLeftFramePos.X, bottomRightFramePos.Y) });

        UpdateSelection(0);
    }

    public MTexture ChooseTexture(int index, bool light)
        => !light ? GFX.Gui[(index % 4) switch
        {
            0 => "SaladimHelper/MaybeAShop/sa",
            1 => "SaladimHelper/MaybeAShop/sb",
            2 => "SaladimHelper/MaybeAShop/sc",
            3 => "SaladimHelper/MaybeAShop/sd",
            _ => null
        }] : GFX.Gui[(index % 4) switch
        {
            0 => "SaladimHelper/MaybeAShop/sal",
            1 => "SaladimHelper/MaybeAShop/sbl",
            2 => "SaladimHelper/MaybeAShop/scl",
            3 => "SaladimHelper/MaybeAShop/sdl",
            _ => null
        }];

    public override void Update()
    {
        base.Update();
        if (Input.MenuRight.Pressed)
        {
            Input.MenuRight.ConsumePress();
            int preIndex = selectedIndex;
            selectedIndex++;
            selectedIndex %= itemsCount;
            UpdateSelection(preIndex);
        }
        if (Input.MenuLeft.Pressed)
        {
            Input.MenuLeft.ConsumePress();
            int preIndex = selectedIndex;
            selectedIndex--;
            selectedIndex = selectedIndex == -1 ? itemsCount - 1 : selectedIndex;
            UpdateSelection(preIndex);
        }
        if (Input.MenuDown.Pressed)
        {
            Input.MenuDown.ConsumePress();
            int preIndex = selectedIndex;
            selectedIndex += lineMax;
            if (selectedIndex >= itemsCount)
                selectedIndex %= lineMax;
            UpdateSelection(preIndex);
        }
        if (Input.MenuUp.Pressed)
        {
            Input.MenuUp.ConsumePress();
            int preIndex = selectedIndex;
            selectedIndex -= lineMax;
            if (selectedIndex < 0)
            {
                selectedIndex = (lineMax + selectedIndex) + (itemRowColumn.rows - 1) * lineMax;
                if (selectedIndex >= itemsCount)
                    selectedIndex -= lineMax;
            }
            UpdateSelection(preIndex);
        }

        if (Input.ESC.Pressed)
        {
            Input.ESC.ConsumePress();
            ended(this);
            RemoveSelf();
        }
        else if (Input.MenuConfirm.Pressed)
        {
            Input.MenuConfirm.ConsumePress();
            bought(this, selectedIndex);
            ended(this);
            RemoveSelf();
        }
        else if (Input.MenuCancel.Pressed)
        {
            Input.MenuCancel.ConsumePress();
            ended(this);
            RemoveSelf();
        }
        else if (SceneAs<Level>().Paused)
        {
            ended(this);
            RemoveSelf();
        }
    }

    public void UpdateSelection(int preIndex)
    {
        items[preIndex].img.Texture = items[preIndex].tex;
        items[selectedIndex].img.Texture = items[selectedIndex].texl;
    }

    public override void Render()
    {
        Vector2 size = itemSize * new Vector2(itemRowColumn.columns, itemRowColumn.rows);
        var topLeft = items[0].img.Position;
        var bottomRight = topLeft + size;
        Draw.Rect(items[0].img.Position, size.X, size.Y, BgColor);

        float l = topLeft.X;
        float r = bottomRight.X;
        // 9 patching
        // TODO
        base.Render();
    }
}