namespace Celeste.Mod.SaladimHelper.Entities;

public class MaybeAShopUI : Entity
{
    public int ItemsCount = 6;

    private Action<MaybeAShopUI> ended;
    private MaybeAShop shop;
    public int selectedIndex = 0;

    public MaybeAShopUI(MaybeAShop shop, Action<MaybeAShopUI> ended)
    {
        this.shop = shop;
        this.ended = ended;
        Tag |= Tags.HUD;
    }

    public override void Update()
    {
        base.Update();
        if (Input.MenuDown.Pressed)
        {
            Input.MenuDown.ConsumePress();
            selectedIndex++;
            selectedIndex %= ItemsCount;
        }
        if (Input.MenuUp.Pressed)
        {
            Input.MenuUp.ConsumePress();
            selectedIndex--;
            selectedIndex = selectedIndex == -1 ? ItemsCount - 1 : selectedIndex;
        }

        if (Input.MenuConfirm.Pressed)
        {
            Input.MenuConfirm.ConsumePress();
            if (selectedIndex == 0)
            {
                Refill r = new(shop.Position, true, true);
                Scene.Add(r);
            }

            RemoveSelf();
            ended?.Invoke(this);
        }
        else if (Input.MenuCancel.Pressed)
        {
            Input.MenuCancel.ConsumePress();
            RemoveSelf();
            ended?.Invoke(this);
        }
    }

    public override void Render()
    {
        base.Render();
        for (int i = 0; i < ItemsCount; i++)
        {
            var c = Color.White;
            if (i == selectedIndex)
                c = Color.Red;
            Draw.HollowRect(new Rectangle(20, 20 + i * (30 + 900 / ItemsCount), 400, 900 / ItemsCount), c);
        }
    }
}