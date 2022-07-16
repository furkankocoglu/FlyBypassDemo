using System;

public static class EventManager
{
    public static event Action  onStartRun, onFinishLevel, onLandAndDance,onOpponentReady,onlevelFailed;
    public static event Action<short> onTurning;
    public static event Action<bool> onChangeCamZoom;
    public static event Action<float> onCharacterMove;
    public static event Action<string> onChangeScore;
    public static void Fire_onTurning(short direction) { onTurning?.Invoke(direction); }
    public static void Fire_onStartRun() { onStartRun?.Invoke(); }
    public static void Fire_onFinishLevel() { onFinishLevel?.Invoke(); }
    public static void Fire_onLandAndDance() { onLandAndDance?.Invoke(); }
    public static void Fire_onChangeCamZoom(bool zoom) { onChangeCamZoom?.Invoke(zoom); }
    public static void Fire_onCharacterMove(float value) { onCharacterMove?.Invoke(value); }
    public static void Fire_onOpponentReady() { onOpponentReady?.Invoke(); }
    public static void Fire_onlevelFailed() { onlevelFailed?.Invoke(); }
    public static void Fire_onChangeScore(string text) { onChangeScore?.Invoke(text); }
}
