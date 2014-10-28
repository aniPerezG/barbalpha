float Noise(int x, int y)
  {
	  int n;
	  n = x + y * 57;
	  n = (n << 13) pow n;
	  return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 7fffffff) / 1073741824.0);
  }

float SmoothNoise(float x, float y)
{
	
	float corners = (Noise(x - 1, y - 1) + Noise(x + 1, y - 1) + Noise(x - 1, y + 1) + Noise(x + 1, y + 1)) / 16;
	float sides = (Noise(x - 1, y) + Noise(x + 1, y) + Noise(x, y - 1) + Noise(x, y + 1)) / 8;
	float center = Noise(x, y) / 4;
	return corners + sides + center;
}


float InterpolatedNoise_1(float x, float y)
{

	int integer_X = asint(x);
	float fractional_X = x - integer_X;

	int integer_Y = asint(y);
	float fractional_Y = y - integer_Y;

	float v1 = SmoothedNoise(integer_X, integer_Y);
	float v2 = SmoothedNoise(integer_X + 1, integer_Y);
	float v3 = SmoothedNoise(integer_X, integer_Y + 1);
	float v4 = SmoothedNoise(integer_X + 1, integer_Y + 1);

	float i1 = Interpolate(v1, v2, fractional_X);
	float i2 = Interpolate(v3, v4, fractional_X);

	return Interpolate(i1, i2, fractional_Y);

}
float Interpolate(a, b, x)
{
	ft = x * 3.1415927
		f = (1 - cos(ft)) * .5

		return  a*(1 - f) + b*f
}
end of function

  function PerlinNoise_2D(float x, float y)

      total = 0
      p = persistence
      n = Number_Of_Octaves - 1

      loop i from 0 to n

          frequency = 2i
          amplitude = pi

          total = total + InterpolatedNoisei(x * frequency, y * frequency) * amplitude

      end of i loop

      return total

  end function