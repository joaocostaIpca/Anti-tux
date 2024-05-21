using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public class Animation
{
    public Texture2D[] Frames { get; private set; }
    public float FrameDuration { get; private set; }
    private float timeSinceLastFrame;
    private int currentFrame;

    public Animation(Texture2D[] frames, float frameDuration)
    {
        Frames = frames;
        FrameDuration = frameDuration;
        currentFrame = 0;
        timeSinceLastFrame = 0f;
    }

    public void Update(float deltaTime)
    {
        timeSinceLastFrame += deltaTime;

        if (timeSinceLastFrame >= FrameDuration)
        {
            currentFrame++;
            if (currentFrame >= Frames.Length)
            {
                currentFrame = 0;
            }
            timeSinceLastFrame = 0f;
        }
    }

    public Texture2D GetCurrentFrame()
    {
        return Frames[currentFrame];
    }
}
