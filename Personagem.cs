using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class Personagem
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsOnGround { get; set; }

    private Dictionary<string, Animation> animations;
    private Animation currentAnimation;
    private string currentAnimationKey;
    public bool isFacingRight;

    public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, currentAnimation.GetCurrentFrame().Width, currentAnimation.GetCurrentFrame().Height);

    public Personagem(Dictionary<string, Animation> animations, Vector2 position)
    {
        this.animations = animations;
        Position = position;
        IsOnGround = false;
        SetAnimation("idle");
        isFacingRight = true;
    }

    public void SetAnimation(string animationKey)
    {
        if (currentAnimationKey != animationKey)
        {
            currentAnimation = animations[animationKey];
            currentAnimationKey = animationKey;
        }
    }

    public void Update(float deltaTime)
    {
        currentAnimation.Update(deltaTime); // Update animation frames
        Position += Velocity * deltaTime;

        // Set default animation if no user input is detected
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        SpriteEffects spriteEffects = isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(currentAnimation.GetCurrentFrame(), Position, null, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
    }

    public int GetCurrentFrameHeight()
    {
        return currentAnimation.GetCurrentFrame().Height;
    }
}
