using LSH.Core;

namespace NightStalker.Core
{
    [SoundIdEnum(SoundType.SFX)]
    public enum SFXID
    {
        None = 0,

        UI_Btn_Click = 100,
        UI_Btn_Hover = 101,
        UI_Btn_Confirm = 102,

        Player_Jump = 200,
        Player_Hit = 201,
    }

    [SoundIdEnum(SoundType.BGM)]
    public enum BGMID
    {
        None = 0,

        Title = 100,
        InGame = 200,
        Result = 300,
    }
}