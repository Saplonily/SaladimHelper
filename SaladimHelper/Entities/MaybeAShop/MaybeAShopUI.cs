using System;

namespace Celeste.Mod.SaladimHelper.Entities;

// 我知道这坨代码写的很烂但是你先别急

public class MaybeAShopUI : Entity
{
    public const float CenterOffsetInX = 160f / 320f;
    public const float CenterOffsetInY = 166f / 466f;

    public const float TextOffsetInX = 105f / 320f;
    public const float TextOffsetInY = 353f / 466f;

    public const float CoinOffsetInX = 200f / 320f;
    public const float CoinOffsetInY = 353f / 466f;

    public readonly static Color BoughtColor = new Color(150, 150, 150);
    public readonly static Color BgColor = Calc.HexToColor("040345");

    private int lineMax;
    private int itemsCount;
    private int selectedIndex = 0;
    private List<(Image img, MTexture tex, MTexture texl)> items;

    private Action<MaybeAShopUI> ended;
    private Action<MaybeAShopUI, int> bought;

    private string[] texs;
    private int[] costs;

    private MTexture t;
    private MTexture b;
    private MTexture l;
    private MTexture r;
    private Vector2 itemSize;
    private (int columns, int rows) itemRowColumn;

    public MaybeAShopUI(Action<MaybeAShopUI, int> bought, Action<MaybeAShopUI> ended, string[] texs, int[] costs, int lineMax)
    {
        //lineMax = 114514;
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
            bool boughtThis = ModuleSession.ShopBoughtItems.Contains(i);
            var tex = ChooseTexture(i, false);
            var texl = ChooseTexture(i, true);
            // 物品框贴图
            Image frameImg = new(tex);
            items.Add((frameImg, tex, texl));
            Vector2 screenCenter = new(Engine.Width / 2, Engine.Height / 2);
            Vector2 framePos = screenCenter + new Vector2(-totalWidth / 2 + curColumn * itemSize.X, -totalHeight / 2 + curRow * itemSize.Y);
            frameImg.Position = framePos;
            Add(frameImg);
            if (boughtThis)
                frameImg.Color = BoughtColor;

            if (i == 0)
                topLeftFramePos = framePos;
            if (bottomRightFramePos.X < framePos.X)
                bottomRightFramePos.X = framePos.X;
            if (bottomRightFramePos.Y < framePos.Y)
                bottomRightFramePos.Y = framePos.Y;

            // 物品贴图
            var texItem = GFX.Gui[this.texs[i]];
            var itemImg = new Image(texItem);
            itemImg.Position = framePos + new Vector2(CenterOffsetInX * frameImg.Width, CenterOffsetInY * frameImg.Height);
            itemImg.CenterOrigin();
            Add(itemImg);
            if (boughtThis)
                itemImg.Color = BoughtColor;

            var coinTex = GFX.Gui["SaladimHelper/coin"];
            var coinImg = new Image(coinTex);
            coinImg.Position = framePos + new Vector2(CoinOffsetInX * frameImg.Width, CoinOffsetInY * frameImg.Height);
            coinImg.Scale = Vector2.One * 0.8f;
            coinImg.CenterOrigin();
            Add(coinImg);
            if (boughtThis)
                coinImg.Color = BoughtColor;

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
            if (TryBuy(selectedIndex))
            {
                bought(this, selectedIndex);
                ended(this);
                RemoveSelf();
            }
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

    private bool TryBuy(int index)
    {
        var cost = costs[index];
        if (ModuleSession.ShopBoughtItems.Contains(index))
            return false;
        if (ModuleSession.CollectedCoinsAmount >= cost)
            return true;
        return false;
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
        // TODO patching in Y direction
        for (float x = l; x < r - itemSize.X; x += itemSize.X)
        {
            //Logger.Log(LogLevel.Info, ModuleName, $"x: {x}, y: {topLeft.Y}");
            t.Draw(new(x, topLeft.Y));
            b.Draw(new(x, bottomRight.Y - itemSize.Y));
        }
        t.Draw(new(r - itemSize.X, topLeft.Y));
        b.Draw(new(r - itemSize.X, bottomRight.Y - itemSize.Y));

        base.Render();
        for (int column = 0; column < itemRowColumn.columns; column++)
            for (int row = 0; row < itemRowColumn.rows; row++)
            {
                int curIndex = column + row * lineMax;
                bool bought = ModuleSession.ShopBoughtItems.Contains(curIndex);
                if (itemsCount <= curIndex)
                    goto @out;
                var cardTopLeft = topLeft + new Vector2(column * itemSize.X, row * itemSize.Y);
                var pos = cardTopLeft + new Vector2(TextOffsetInX * itemSize.X, TextOffsetInY * itemSize.Y);
                Color color = bought ? BoughtColor : Color.White;
                ActiveFont.DrawOutline($"x{costs[curIndex]}", pos, new Vector2(0f, 0.5f), Vector2.One, color, 1f, Color.Black);
            }
        @out:
        return;
    }
}