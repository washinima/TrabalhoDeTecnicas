using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windoes_Size_Project
{
    public class GameState
    {
        // Work with TexturedPrimitive
        TexturedPrimitive mBall, mUWBLogo;
        TexturedPrimitive mWorkPrim;

        // Size of all the positions
        Vector2 kPointSize = new Vector2(5f, 5f);
        // Work with TexturedPrimitive
        TexturedPrimitive mPa, mPb; // The locators for showing Point A and Point B
        TexturedPrimitive mPx; // to show same displacement can be applied to any position
        TexturedPrimitive mPy; // To show we can rotate/manipulate vectors independently
        Vector2 mVectorAtPy = new Vector2(10, 0); // Start with vector in the X direction;
        TexturedPrimitive mCurrentLocator;

        // Rocket support
        Vector2 mRocketInitDirection = Vector2.UnitY; // This does not change
        TexturedPrimitive mRocket;
        // Support the flying net
        TexturedPrimitive mNet;
        bool mNetInFlight = false;
        Vector2 mNetVelocity = Vector2.Zero;
        float mNetSpeed = 0.5f;
        // Insect support
        TexturedPrimitive mInsect;
        bool mInsectPreset = true;
        // Simple game status
        int mNumInsectShot;
        int mNumMissed;

        Vector2 kInitRocketPosition = new Vector2(10, 10);
        // Rocket support
        GameObject mrocket;
        // The arrow
        GameObject mArrow;

        ChaserGameObject mChaser;
        // Simple game status
        int mChaserHit, mChaserMissed;

        public GameState()
        {
            // Create the primitives
            mBall = new TexturedPrimitive("Soccer", new Vector2(30, 30), new Vector2(10, 15));
            mUWBLogo = new TexturedPrimitive("UWB-JPG", new Vector2(60, 30), new Vector2(20, 20));
            mWorkPrim = mBall;

            // Create the primitives
            mPa = new TexturedPrimitive("Position", new Vector2(30, 30), kPointSize, "Pa");
            mPb = new TexturedPrimitive("Position", new Vector2(60, 30), kPointSize, "Pb");
            mPx = new TexturedPrimitive("Position", new Vector2(20, 10), kPointSize, "Px");
            mPy = new TexturedPrimitive("Position", new Vector2(20, 50), kPointSize, "Py");
            mCurrentLocator = mPa;

            // Create and set up the primitives
            mRocket = new TexturedPrimitive("Rocket", new Vector2(5, 5), new Vector2(3, 10));
            // Initially the rocket is pointing in the positive y direction
            mRocketInitDirection = Vector2.UnitY;
            mNet = new TexturedPrimitive("Net", new Vector2(0, 0), new Vector2(2, 5));
            mNetInFlight = false; // until user press "A", rocket is not in flight
            mNetVelocity = Vector2.Zero;
            mNetSpeed = 0.5f;
            // Initialize a new insect
            mInsect = new TexturedPrimitive("Insect", Vector2.Zero, new Vector2(5, 5));
            mInsectPreset = false;
            // Initialize game status
            mNumInsectShot = 0;
            mNumMissed = 0;

            mrocket = new GameObject("Rocket", kInitRocketPosition, new Vector2(3, 10));
            mArrow = new GameObject("RightArrow", new Vector2(50, 30), new Vector2(10, 4));
            // Initially pointing in the x direction
            mArrow.InitialFrontDirection = Vector2.UnitX;

            mChaser = new ChaserGameObject("Chaser", Vector2.Zero, new Vector2(6f, 1.7f), null);
            // Initially facing in the negative x direction
            mChaser.InitialFrontDirection = -Vector2.UnitX;
            mChaser.Speed = 0.2f;
            mChaserHit = 0;
            mChaserMissed = 0;
        }

        public void UpdateGame()
        {
            #region Select which primitive to work on
            if (InputWrapper.Buttons.A == ButtonState.Pressed)
                mWorkPrim = mBall;
            else if (InputWrapper.Buttons.B == ButtonState.Pressed)
                mWorkPrim = mUWBLogo;
            #endregion

            #region Update the work primitive
            float rotation = 0;
            if (InputWrapper.Buttons.X == ButtonState.Pressed)
                rotation = MathHelper.ToRadians(1f); // 1 degree pre-press
            else if (InputWrapper.Buttons.Y == ButtonState.Pressed)
                rotation = MathHelper.ToRadians(-1f); // 1 degree pre-press
            mWorkPrim.Update(
            InputWrapper.ThumbSticks.Left,
            InputWrapper.ThumbSticks.Right,
            rotation);
            #endregion

            #region Step 3a. Change current selected vector
            if (InputWrapper.Buttons.A == ButtonState.Pressed)
                mCurrentLocator = mPa;
            else if (InputWrapper.Buttons.B == ButtonState.Pressed)
                mCurrentLocator = mPb;
            else if (InputWrapper.Buttons.X == ButtonState.Pressed)
                mCurrentLocator = mPx;
            else if (InputWrapper.Buttons.Y == ButtonState.Pressed)
                mCurrentLocator = mPy;
            #endregion

            #region Step 3b. Move Vector
            // Change the current locator position
            mCurrentLocator.mPosition += InputWrapper.ThumbSticks.Right;
            #endregion

            #region Step 3c. Rotate Vector
            // Left thumbstick-X rotates the vector at Py
            float rotateYByRadian = MathHelper.ToRadians(
                        InputWrapper.ThumbSticks.Left.X);
            #endregion

            #region Step 3d. Increase/Decrease the length of vector
            // Left thumbstick-Y increase/decrease the length of vector at Py
            float vecYLen = mVectorAtPy.Length();
            vecYLen += InputWrapper.ThumbSticks.Left.Y;
            #endregion

            #region Step 3e. Compute vector changes
            // Compute the rotated direction of vector at Py
            mVectorAtPy = ShowVector.RotateVectorByAngle(mVectorAtPy, rotateYByRadian);
            mVectorAtPy.Normalize(); // Normalize vectorATY to size of 1f
            mVectorAtPy *= vecYLen;  // Scale the vector to the new size
            #endregion

            #region Step 4a. Update Rocket control
            mRocket.RotateAngleInRadian += MathHelper.ToRadians(InputWrapper.ThumbSticks.Right.X);
            mRocket.mPosition += InputWrapper.ThumbSticks.Left;
            #endregion

            #region Step 4b. Update net in flight
            /// Set net to flight
            if (InputWrapper.Buttons.A == ButtonState.Pressed)
            {
                mNetInFlight = true;
                mNet.RotateAngleInRadian = mRocket.RotateAngleInRadian;
                mNet.mPosition = mRocket.mPosition;
                mNetVelocity = ShowVector.RotateVectorByAngle(
                                mRocketInitDirection, mNet.RotateAngleInRadian) * mNetSpeed;
            }
            #endregion

            #region Step 4c. Update bee respawn
            if (!mInsectPreset)
            {
                float x = 15f + ((float)Game1.sRan.NextDouble() * 30f);
                float y = 15f + ((float)Game1.sRan.NextDouble() * 30f);
                mInsect.mPosition = new Vector2(x, y);
                mInsectPreset = true;
            }
            #endregion

            #region Step 5. Inter-object interaction: Net in flight and collision with insect
            if (mNetInFlight)
            {
                mNet.mPosition += mNetVelocity;

                if (mNet.PrimitivesTouches(mInsect))
                {
                    mInsectPreset = false;
                    mNetInFlight = false;
                    mNumInsectShot++;
                }
                if ((Camera.CollidedWithCameraWindow(mNet) !=
                        Camera.CameraWindowCollisionStatus.InsideWindow))
                {
                    mNetInFlight = false;
                    mNumMissed++;
                }
            }
            #endregion

            #region Step 3a. Control and fly the rocket
            mrocket.RotateAngleInRadian +=MathHelper.ToRadians(InputWrapper.ThumbSticks.Right.X);

            mrocket.Speed += InputWrapper.ThumbSticks.Left.Y * 0.1f;

            mrocket.VelocityDirection = mrocket.FrontDirection;

            if (Camera.CollidedWithCameraWindow(mRocket) !=
                        Camera.CameraWindowCollisionStatus.InsideWindow)
            {
                mrocket.Speed = 0f;
                mrocket.mPosition = kInitRocketPosition;
            }

            mrocket.Update();
            #endregion

            #region Step 3b. Set the arrow to point towards the rocket
            Vector2 toRocket = mrocket.mPosition - mArrow.mPosition;
            mArrow.FrontDirection = toRocket;
            #endregion

            #region 3. Check/launch the chaser!
            if (mChaser.HasValidTarget)
            {
                mChaser.ChaseTarget();
                if (mChaser.HitTarget)
                {
                    mChaserHit++;
                    mChaser.Target = null;
                }
                if (Camera.CollidedWithCameraWindow(mChaser) !=
                Camera.CameraWindowCollisionStatus.InsideWindow)
                {
                    mChaserMissed++;
                    mChaser.Target = null;
                }
            }
            if (InputWrapper.Buttons.A == ButtonState.Pressed)
            {
                mChaser.Target = mrocket;
                mChaser.mPosition = mArrow.mPosition;
            }
            #endregion
        }

        public void DrawGame()
        {
            mBall.Draw();
            FontSupport.PrintStatusAt(mBall.mPosition, mBall.RotateAngleInRadian.ToString(), Color.Red);

            mUWBLogo.Draw();
            FontSupport.PrintStatusAt(mUWBLogo.mPosition, mUWBLogo.mPosition.ToString(), Color.Black);

            // Print out text message to echo status
            FontSupport.PrintStatus("A-Soccer B-Logo LeftThumb:Move RightThumb:Scale X/Y:Rotate", null);

            // Drawing the vectors
            Vector2 v = mPb.mPosition - mPa.mPosition;    // Vector V is from Pa to Pb

            // Draw Vector-V at Pa, and Px
            ShowVector.DrawFromTo(mPa.mPosition, mPb.mPosition);
            ShowVector.DrawPointVector(mPx.mPosition, v);

            // Draw VectorAtY at Py
            ShowVector.DrawPointVector(mPy.mPosition, mVectorAtPy);

            mPa.Draw();
            mPb.Draw();
            mPx.Draw();
            mPy.Draw();

            // Print out text message to echo status
            FontSupport.PrintStatus("Locator Positions: A=" + mPa.mPosition + "  B=" + mPb.mPosition, null);

            mRocket.Draw();
            if (mNetInFlight)
                mNet.Draw();

            if (mInsectPreset)
                mInsect.Draw();

            // Print out text messsage to echo status
            FontSupport.PrintStatus("Num insects netted=" + mNumInsectShot + "  Num missed=" + mNumMissed, null);

            mrocket.Draw();
            mArrow.Draw();
            // print out text message to echo status
            FontSupport.PrintStatus("Rocket Speed(LeftThumb-Y)=" + mrocket.Speed +" VelocityDirection(RightThumb-X):" + mrocket.VelocityDirection, null);
            FontSupport.PrintStatusAt(mrocket.mPosition, mrocket.mPosition.ToString(), Color.White);

            mrocket.Draw();
            mArrow.Draw();
            if (mChaser.HasValidTarget)
                mChaser.Draw();
            // Print out text message to echo status
            FontSupport.PrintStatus("Chaser Hit=" + mChaserHit + " Missed=" + mChaserMissed, null);
        }
    }
}
