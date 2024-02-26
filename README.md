# PathTracing
My second attempt at making a graphics engine. This time, I chose to use C#.
Rendring runs on the CPU, but on all cores, which makes it just a bit faster.
There is support for Sheres and STL files (the binary ones).
<br>
## Download
```bash
git clone https://github.com/Stachu1/PathTracing.git
```

<br>

## Usage
**1. Open with VS**
<br>
**2. Compile**
<br>
**3. Be sure to move the "Scene" folder to the same location as the main executable (PathTracing\PathTracing\bin\Debug\net7.0-windows\)**
<br>
**4. Restart the program**
<br>
**5. RENDER!**

<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/3ad37c9a-19c6-45c5-ba97-7163531e7d88">

<br>

## Configuration
All the configuraton can be done within four files: Camera.json, Materials.json, Spheres.json, STLs.json
<br>
<br>
**Camera**
```json
{
  "pos": [ 0.0, 37.0, -40.0 ],
  "rotation": [ 0.0, -29.0, 0.0 ],
  "resolution": [ 600, 600 ],
  "FOV": 50.0,
  "gamma": 1.6,
  "ray_deviation": 0.002,
  "samples_per_pixel": 10,
  "SPP_multiplier_for_transparent_materials":  10.0
}
```
The last value allows you to set a multiplier for transparent objects, as they generally need more samples to look good.

<br>

**Materials**
<br>
Specular reflection gives a glazing effect.
<br>
Refractive index controls how much light is refracted in the material (only for transparent materials).
```json
[
  {
    "name": "smooth_green",
    "color": [
      0.1,
      0.8,
      0.1
    ],
    "smoothness": 0,
    "specular_reflection_probability": 0.1,
    "transparency": 0,
    "refractive_index": 1,
    "light_emission": 0
  }
]
```

<br>

**Spheres**
```json
[
 {
    "pos": [
      0,
      6,
      0
    ],
    "radius": 6,
    "material_name": "smooth_green"
  }
]
```

<br>

**STLs**
```json
[
  {
    "pos": [
      12,
      6,
      -6
    ],
    "rotation": [
      -5,
      0,
      0
    ],
    "scale": 0.6,
    "path": "Scene/STLs/pyramid.stl",
    "material_name": "mirror"
  }
]
```
<br>

## Renders
<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/8a031b9c-66bb-4131-9bf7-033325c26c91">
<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/b1977560-647a-459c-a6eb-98d0560f09a8">
<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/15fff91f-db3a-4dd8-a24e-010937f6a85c">
<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/8b937ce1-1b98-47ad-a8a4-a2fc81586df6">
<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/ce1681bf-1131-4a0c-8653-8107caa708ff">
<img width="1000" alt="image" src="https://github.com/Stachu1/PathTracing/assets/77758413/92131b41-48b9-4535-896e-7c4f86521ff5">
