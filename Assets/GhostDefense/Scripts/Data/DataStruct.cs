namespace UDEV.GhostDefense
{
    public enum GameTag
    {
        Player,
        Enemy,
        Collectable
    }

    public enum GameScene
    {
        Gameplay,
        MainMenu
    }

    public enum KeyPref
    {
        game_data_,
        IsFirstTime,
        SpriteOrder,
        CloudDataLoaded
    }

    public enum Direction
    {
        Left, Right, Top, Bottom, None
    }

    public enum GameState
    {
        Starting,
        Playing,
        Wining,
        Gameover
    }

    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Dash,
        Attack,
        Ultimate,
        GotHit,
        Dead
    }

    public enum AIState
    {
        Walk,
        Dash1,
        Dash2,
        Attack,
        GotHit,
        Ultimate,
        Dead
    }

    public enum PlayerCollider
    {
        Normal,
        Dead
    }

    public enum FBLogEvent
    {
        hero_shop_open,
        iap_shop_open,
        gameover_ads,
        mission_completed_ads,
        revive_ads,
        iap_no_ads
    }
}