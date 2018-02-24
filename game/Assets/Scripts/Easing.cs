// from: http://www.robertpenner.com/easing/

// Removed animation stuff - not needed for Unity.
// Fixed documentation erroneously calling param "c" Final value. If c == 0 then fuck all happens. Therefore:
// c is not the final value and the person who wrote that it is, happens to be an asshole that wasted me an hour
// figuring out why.

using UnityEngine;
using System;

/// <summary>
/// Animates the change in value of a double property using 
/// Robert Penner's easing equations for interpolation over a specified duration.
/// </summary>
public class Easing{
	
	/// <summary>
	/// Go forth then double back to start after halfway.
	/// Useful when assigning delegates to control animation. 
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double Nope( double t, double b, double c, double d )
	{
		return c * (t > d * 0.5 ? d - t : t) / d + b;
	}
	/// <summary>
	/// As Nope, but faster in, slower out
	/// Useful when assigning delegates to control animation. 
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double Stab( double t, double b, double c, double d )
	{
		return c * (t > d * 0.25 ? d - t : t * 4) / d + b;
	}
	/// <summary>
	/// Linear change in value.
	/// Useful when assigning delegates to control animation. 
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double Linear( double t, double b, double c, double d )
	{
		return c * t / d + b;
	}
	
	/// <summary>
	/// Easing equation function for an exponential (2^t) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ExpoEaseOut( double t, double b, double c, double d )
	{
		return ( t == d ) ? b + c : c * ( -Math.Pow( 2, -10 * t / d ) + 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for an exponential (2^t) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ExpoEaseIn( double t, double b, double c, double d )
	{
		return ( t == 0 ) ? b : c * Math.Pow( 2, 10 * ( t / d - 1 ) ) + b;
	}
	
	/// <summary>
	/// Easing equation function for an exponential (2^t) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ExpoEaseInOut( double t, double b, double c, double d )
	{
		if ( t == 0 )
			return b;
		
		if ( t == d )
			return b + c;
		
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * Math.Pow( 2, 10 * ( t - 1 ) ) + b;
		
		return c / 2 * ( -Math.Pow( 2, -10 * --t ) + 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for an exponential (2^t) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ExpoEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return ExpoEaseOut( t * 2, b, c / 2, d );
		
		return ExpoEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CircEaseOut( double t, double b, double c, double d )
	{
		return c * Math.Sqrt( 1 - ( t = t / d - 1 ) * t ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CircEaseIn( double t, double b, double c, double d )
	{
		return -c * ( Math.Sqrt( 1 - ( t /= d ) * t ) - 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CircEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return -c / 2 * ( Math.Sqrt( 1 - t * t ) - 1 ) + b;
		
		return c / 2 * ( Math.Sqrt( 1 - ( t -= 2 ) * t ) + 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CircEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return CircEaseOut( t * 2, b, c / 2, d );
		
		return CircEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuadEaseOut( double t, double b, double c, double d )
	{
		return -c * ( t /= d ) * ( t - 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuadEaseIn( double t, double b, double c, double d )
	{
		return c * ( t /= d ) * t + b;
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuadEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * t * t + b;
		
		return -c / 2 * ( ( --t ) * ( t - 2 ) - 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quadratic (t^2) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuadEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return QuadEaseOut( t * 2, b, c / 2, d );
		
		return QuadEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a sinusoidal (sin(t)) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double SineEaseOut( double t, double b, double c, double d )
	{
		return c * Math.Sin( t / d * ( Math.PI / 2 ) ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a sinusoidal (sin(t)) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double SineEaseIn( double t, double b, double c, double d )
	{
		return -c * Math.Cos( t / d * ( Math.PI / 2 ) ) + c + b;
	}
	
	/// <summary>
	/// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double SineEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * ( Math.Sin( Math.PI * t / 2 ) ) + b;
		
		return -c / 2 * ( Math.Cos( Math.PI * --t / 2 ) - 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double SineEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return SineEaseOut( t * 2, b, c / 2, d );
		
		return SineEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a cubic (t^3) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CubicEaseOut( double t, double b, double c, double d )
	{
		return c * ( ( t = t / d - 1 ) * t * t + 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a cubic (t^3) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CubicEaseIn( double t, double b, double c, double d )
	{
		return c * ( t /= d ) * t * t + b;
	}
	
	/// <summary>
	/// Easing equation function for a cubic (t^3) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CubicEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * t * t * t + b;
		
		return c / 2 * ( ( t -= 2 ) * t * t + 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a cubic (t^3) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double CubicEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return CubicEaseOut( t * 2, b, c / 2, d );
		
		return CubicEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a quartic (t^4) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuartEaseOut( double t, double b, double c, double d )
	{
		return -c * ( ( t = t / d - 1 ) * t * t * t - 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quartic (t^4) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuartEaseIn( double t, double b, double c, double d )
	{
		return c * ( t /= d ) * t * t * t + b;
	}
	
	/// <summary>
	/// Easing equation function for a quartic (t^4) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuartEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * t * t * t * t + b;
		
		return -c / 2 * ( ( t -= 2 ) * t * t * t - 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quartic (t^4) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuartEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return QuartEaseOut( t * 2, b, c / 2, d );
		
		return QuartEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a quintic (t^5) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuintEaseOut( double t, double b, double c, double d )
	{
		return c * ( ( t = t / d - 1 ) * t * t * t * t + 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quintic (t^5) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuintEaseIn( double t, double b, double c, double d )
	{
		return c * ( t /= d ) * t * t * t * t + b;
	}
	
	/// <summary>
	/// Easing equation function for a quintic (t^5) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuintEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * t * t * t * t * t + b;
		return c / 2 * ( ( t -= 2 ) * t * t * t * t + 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a quintic (t^5) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double QuintEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return QuintEaseOut( t * 2, b, c / 2, d );
		return QuintEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ElasticEaseOut( double t, double b, double c, double d )
	{
		if ( ( t /= d ) == 1 )
			return b + c;
		
		double p = d * .3;
		double s = p / 4;
		
		return ( c * Math.Pow( 2, -10 * t ) * Math.Sin( ( t * d - s ) * ( 2 * Math.PI ) / p ) + c + b );
	}
	
	/// <summary>
	/// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ElasticEaseIn( double t, double b, double c, double d )
	{
		if ( ( t /= d ) == 1 )
			return b + c;
		
		double p = d * .3;
		double s = p / 4;
		
		return -( c * Math.Pow( 2, 10 * ( t -= 1 ) ) * Math.Sin( ( t * d - s ) * ( 2 * Math.PI ) / p ) ) + b;
	}
	
	/// <summary>
	/// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ElasticEaseInOut( double t, double b, double c, double d )
	{
		if ( ( t /= d / 2 ) == 2 )
			return b + c;
		
		double p = d * ( .3 * 1.5 );
		double s = p / 4;
		
		if ( t < 1 )
			return -.5 * ( c * Math.Pow( 2, 10 * ( t -= 1 ) ) * Math.Sin( ( t * d - s ) * ( 2 * Math.PI ) / p ) ) + b;
		return c * Math.Pow( 2, -10 * ( t -= 1 ) ) * Math.Sin( ( t * d - s ) * ( 2 * Math.PI ) / p ) * .5 + c + b;
	}
	
	/// <summary>
	/// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double ElasticEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return ElasticEaseOut( t * 2, b, c / 2, d );
		return ElasticEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BounceEaseOut( double t, double b, double c, double d )
	{
		if ( ( t /= d ) < ( 1 / 2.75 ) )
			return c * ( 7.5625 * t * t ) + b;
		else if ( t < ( 2 / 2.75 ) )
			return c * ( 7.5625 * ( t -= ( 1.5 / 2.75 ) ) * t + .75 ) + b;
		else if ( t < ( 2.5 / 2.75 ) )
			return c * ( 7.5625 * ( t -= ( 2.25 / 2.75 ) ) * t + .9375 ) + b;
		else
			return c * ( 7.5625 * ( t -= ( 2.625 / 2.75 ) ) * t + .984375 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BounceEaseIn( double t, double b, double c, double d )
	{
		return c - BounceEaseOut( d - t, 0, c, d ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BounceEaseInOut( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return BounceEaseIn( t * 2, 0, c, d ) * .5 + b;
		else
			return BounceEaseOut( t * 2 - d, 0, c, d ) * .5 + c * .5 + b;
	}
	
	/// <summary>
	/// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BounceEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return BounceEaseOut( t * 2, b, c / 2, d );
		return BounceEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
	
	/// <summary>
	/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
	/// decelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BackEaseOut( double t, double b, double c, double d )
	{
		return c * ( ( t = t / d - 1 ) * t * ( ( 1.70158 + 1 ) * t + 1.70158 ) + 1 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
	/// accelerating from zero velocity.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BackEaseIn( double t, double b, double c, double d )
	{
		return c * ( t /= d ) * t * ( ( 1.70158 + 1 ) * t - 1.70158 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
	/// acceleration until halfway, then deceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BackEaseInOut( double t, double b, double c, double d )
	{
		double s = 1.70158;
		if ( ( t /= d / 2 ) < 1 )
			return c / 2 * ( t * t * ( ( ( s *= ( 1.525 ) ) + 1 ) * t - s ) ) + b;
		return c / 2 * ( ( t -= 2 ) * t * ( ( ( s *= ( 1.525 ) ) + 1 ) * t + s ) + 2 ) + b;
	}
	
	/// <summary>
	/// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
	/// deceleration until halfway, then acceleration.
	/// </summary>
	/// <param name="t">Current time in seconds.</param>
	/// <param name="b">Starting value.</param>
	/// <param name="c">Change in value.</param>
	/// <param name="d">Duration of animation.</param>
	/// <returns>The correct value.</returns>
	public static double BackEaseOutIn( double t, double b, double c, double d )
	{
		if ( t < d / 2 )
			return BackEaseOut( t * 2, b, c / 2, d );
		return BackEaseIn( ( t * 2 ) - d, b + c / 2, c / 2, d );
	}
}

/* Basic point to point tween */
public class Travel {
	public Vector3 start;
	public Vector3 dest;
	public Transform transform;
	// supply a function from Easing.cs
	public delegate double Ease(double t, double b, double c, double d);
	public Ease ease;
	public Travel(Transform transform, Vector3 start, Vector3 dest, Ease ease) {
		this.start = start;
		this.dest = dest;
		this.ease = ease;
		this.transform = transform;
	}
	/* t is a value from 0 to 1, the position is then applied to the transform */
	public void SetDelta(float t) {
		if(t > 1) t = 1;
		Vector3 tempVec = transform.localPosition;
		if(start.x != dest.x) {
			tempVec.x = Mathf.Round((float)ease(t, start.x, dest.x - start.x, 1));
		}
		if(start.y != dest.y) {
			tempVec.y = Mathf.Round((float)ease(t, start.y, dest.y - start.y, 1));
		}
		transform.localPosition = tempVec;
	}
}
