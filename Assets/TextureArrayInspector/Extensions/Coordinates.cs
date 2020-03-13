using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection; //to copy properties

namespace TextureArrayInspector
{
	static public class CoordinatesExtensions
	{
		public static bool InRange (this Rect rect, Vector2 pos) 
		{ 
			return (rect.center - pos).sqrMagnitude < (rect.width/2f)*(rect.width/2f); 
			//return rect.Contains(pos);
		}

		public static Vector3 ToDir (this float angle) { return new Vector3( Mathf.Sin(angle*Mathf.Deg2Rad), 0, Mathf.Cos(angle*Mathf.Deg2Rad) ); }
		public static float ToAngle (this Vector3 dir) { return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg; }

		public static Vector3 V3 (this Vector2 v2) { return new Vector3(v2.x, 0, v2.y); }
		public static Vector2 V2 (this Vector3 v3) { return new Vector2(v3.x, v3.z); }
		public static Vector3 ToV3 (this float f) { return new Vector3(f,f, f); }

		public static Quaternion EulerToQuat (this Vector3 v) { Quaternion rotation = Quaternion.identity; rotation.eulerAngles = v; return rotation; }
		public static Quaternion EulerToQuat (this float f) { Quaternion rotation = Quaternion.identity; rotation.eulerAngles = new Vector3(0,f,0); return rotation; }

		public static Vector3 Direction (this float angle) { return new Vector3( Mathf.Sin(angle*Mathf.Deg2Rad), 0, Mathf.Cos(angle*Mathf.Deg2Rad) ); }
		public static float Angle (this Vector3 dir) { return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg; }

		public static Rect Clamp (this Rect r, float p) { return new Rect(r.x, r.y, r.width*p, r.height); }
		public static Rect ClampFromLeft (this Rect r, float p) { return new Rect(r.x+r.width*(1f-p), r.y, r.width*p, r.height); }
		public static Rect Clamp (this Rect r, int p) { return new Rect(r.x, r.y, p, r.height); }
		public static Rect ClampFromLeft (this Rect r, int p) { return new Rect(r.x+(r.width-p), r.y, p, r.height); }

		public static Rect Intersect (Rect r1, Rect r2) 
		{ 
			Rect result = new Rect(0,0,0,0);

			result.x = Mathf.Max(r1.x, r2.x);
			result.y = Mathf.Max(r1.y, r2.y);

			result.max = new Vector2(
				Mathf.Min(r1.max.x, r2.max.x),
				Mathf.Min(r1.max.y, r2.max.y) );
			
			if (result.size.x<0) result.size = new Vector2(0, result.size.y);
			if (result.size.y<0) result.size = new Vector2(result.size.y, 0);

			return result;
		}

		public static Rect Intersect (Rect r1, CoordRect r2) 
		{ 
			Rect result = new Rect(0,0,0,0);

			result.x = Mathf.Max(r1.x, r2.offset.x);
			result.y = Mathf.Max(r1.y, r2.offset.z);

			result.max = new Vector2(
				Mathf.Min(r1.max.x, r2.offset.x+r2.size.x),
				Mathf.Min(r1.max.y, r2.offset.z+r2.size.z) );
			
			if (result.size.x<0) result.size = new Vector2(0, result.size.y);
			if (result.size.y<0) result.size = new Vector2(result.size.y, 0);

			return result;
		}


		public static Rect ToRect(this Vector3 center, float range) { return new Rect(center.x-range, center.z-range, range*2, range*2); }

		public static Vector3 Average (this Vector3[] vecs) { Vector3 result = Vector3.zero; for (int i=0; i<vecs.Length; i++) result+=vecs[i]; return result / vecs.Length; }

		public static bool Intersects (this Rect r1, Rect r2)
		{
			Vector2 r1min = r1.min; Vector2 r1max = r1.max;
			Vector2 r2min = r2.min; Vector2 r2max = r2.max;
			if (r2max.x < r1min.x || r2min.x > r1max.x || r2max.y < r1min.y || r2min.y > r1max.y) return false;
			else return true;
		}
		public static bool Intersects (this Rect r1, Rect[] rects)
		{
			for (int i=0; i<rects.Length; i++) 
				if (r1.Intersects(rects[i])) return true; //todo: remove fn call, use r1 min max
			return false;
		}

		public static bool Contains (this Rect r1, Rect r2)
		{
			Vector2 r1min = r1.min; Vector2 r1max = r1.max;
			Vector2 r2min = r2.min; Vector2 r2max = r2.max;
			if (r2min.x > r1min.x && r2max.x < r1max.x && r2min.y > r1min.y && r2max.y < r1max.y) return true;
			else return false;
		}

		public static Rect Extend (this Rect r, float f) { return new Rect(r.x-f, r.y-f, r.width+f*2, r.height+f*2); }

		public static float DistToRectCenter (this Vector3 pos, float offsetX, float offsetZ, float size)
		{
			//TODO: evaluating distance to center, should evaluate distance to rect edges isntead
			//float posX = pos.x; float posZ = pos.z;
			float deltaX = pos.x - (offsetX+size/2); float deltaZ = pos.z - (offsetZ+size/2); float dist = deltaX*deltaX + deltaZ*deltaZ;

			/*float deltaBX = pos.x - offsetX; float deltaBZ = pos.z - offsetZ; float distB = deltaBX*deltaBX + deltaBZ*deltaBZ;
			float deltaCX = pos.x - (offsetX+size); float deltaCZ = pos.z - (offsetZ+size); float distC = deltaCX*deltaCX + deltaCZ*deltaCZ;
			float deltaDX = pos.x - (offsetX+size); float deltaDZ = pos.z - (offsetZ+size); float distD = deltaDX*deltaDX + deltaDZ*deltaDZ;

			//zero dist if pos inside rect
			if (deltaAX>0 && deltaAZ>0 && 

			float minAB = distA<distB? distA : distB;
			float minCD = distC<distD? distC : distD;
			float minABCD = minAB<minCD? minAB : minCD;*/

			return Mathf.Sqrt(dist);
		}

		public static float DistToRectAxisAligned (this Vector3 pos, float offsetX, float offsetZ, float size) //NOT manhattan dist. offset and size are instead of UnityEngine.Rect
		{
			//finding x distance
			float distPosX = offsetX - pos.x;
			float distNegX = pos.x - offsetX - size;
			
			float distX;
			if (distPosX >= 0) distX = distPosX;
			else if (distNegX >= 0) distX = distNegX;
			else distX = 0;

			//finding z distance
			float distPosZ = offsetZ - pos.z;
			float distNegZ = pos.z - offsetZ - size;
			
			float distZ;
			if (distPosZ >= 0) distZ = distPosZ;
			else if (distNegZ >= 0) distZ = distNegZ;
			else distZ = 0;

			//returning the maximum(!) distance 
			if (distX > distZ) return distX;
			else return distZ;
		}



		public static float DistToRectCenter (this Vector3[] poses, float offsetX, float offsetZ, float size)
		{
			float minDist = 200000000;
			for (int p=0; p<poses.Length; p++)
			{
				float dist = poses[p].DistToRectCenter(offsetX, offsetZ, size);
				if (dist < minDist) minDist = dist;
			}
			return minDist;
		}

		public static float DistToRectAxisAligned (this Vector3[] poses, float offsetX, float offsetZ, float size)
		{
			float minDist = 200000000;
			for (int p=0; p<poses.Length; p++)
			{
				float dist = poses[p].DistToRectAxisAligned(offsetX, offsetZ, size);
				if (dist < minDist) minDist = dist;
			}
			return minDist;
		}

		public static float DistAxisAligned (this Vector3 center, Vector3 pos)
		{
			float distX = center.x - pos.x; if (distX<0) distX = -distX;
			float distZ = center.z - pos.z; if (distZ<0) distZ = -distZ;

			//returning the maximum(!) distance 
			if (distX > distZ) return distX;
			else return distZ;
		}



		/*public static Coord ToCoord (this Vector3 pos, float cellSize, bool ceil=false) //to use in object grid
		{
			if (!ceil) return new Coord(
				Mathf.FloorToInt((pos.x) / cellSize),
				Mathf.FloorToInt((pos.z) / cellSize) ); 
			else return new Coord(
				Mathf.CeilToInt((pos.x) / cellSize),
				Mathf.CeilToInt((pos.z) / cellSize) ); 
		}*/

		public static Coord RoundToCoord (this Vector2 pos) //to use in spatial hash (when sphash and matrix sizes are equal)
		{
			int posX = (int)(pos.x + 0.5f); if (pos.x < 0) posX--; //snippet for RoundToInt
			int posZ = (int)(pos.y + 0.5f); if (pos.y < 0) posZ--;
			return new Coord(posX, posZ);
		}

		public static Coord FloorToCoord (this Vector3 pos, float cellSize) { return new Coord( Mathf.FloorToInt(pos.x / cellSize),		Mathf.FloorToInt(pos.z / cellSize)  ); }
		public static Coord CeilToCoord (this Vector3 pos, float cellSize) { return new Coord( Mathf.CeilToInt(pos.x / cellSize),		Mathf.CeilToInt(pos.z / cellSize)  ); }
		public static Coord RoundToCoord (this Vector3 pos, float cellSize) { return new Coord( Mathf.RoundToInt(pos.x / cellSize),		Mathf.RoundToInt(pos.z / cellSize)  ); }
		public static CoordRect ToCoordRect (this Vector3 pos, float range, float cellSize) //this one works with Terrain Sculptor
		{
			Coord min = new Coord( Mathf.FloorToInt((pos.x-range)/cellSize),	Mathf.FloorToInt((pos.z-range)/cellSize)  );
			Coord max = new Coord( Mathf.FloorToInt((pos.x+range)/cellSize),	Mathf.FloorToInt((pos.z+range)/cellSize)  )  +  1;
			return new CoordRect(min, max-min);
		}

		public static CoordRect ToFixedSizeCoordRect (this Vector3 pos, float range, float cellSize)
		{
			Coord size = (Vector3.one*range*2).CeilToCoord(cellSize) + 1;
			Coord offset = pos.RoundToCoord(cellSize) - size/2;
			return new CoordRect (offset, size);
		}

		public static CoordRect GetHeightRect (this Terrain terrain) 
		{
			float pixelSize = terrain.terrainData.size.x / terrain.terrainData.heightmapResolution;

			int posX = (int)(terrain.transform.localPosition.x/pixelSize + 0.5f); if (terrain.transform.localPosition.x < 0) posX--;
			int posZ = (int)(terrain.transform.localPosition.z/pixelSize + 0.5f); if (terrain.transform.localPosition.z < 0) posZ--;

			return new CoordRect(posX, posZ, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
		}

		public static CoordRect GetSplatRect (this Terrain terrain) 
		{
			float pixelSize = terrain.terrainData.size.x / terrain.terrainData.alphamapResolution;

			int posX = (int)(terrain.transform.localPosition.x/pixelSize + 0.5f); if (terrain.transform.localPosition.x < 0) posX--;
			int posZ = (int)(terrain.transform.localPosition.z/pixelSize + 0.5f); if (terrain.transform.localPosition.z < 0) posZ--;

			return new CoordRect(posX, posZ, terrain.terrainData.alphamapResolution, terrain.terrainData.alphamapResolution);
		}

		public static float[,] SafeGetHeights (this TerrainData data, int offsetX, int offsetZ, int sizeX, int sizeZ)
		{
			if (offsetX<0) { sizeX += offsetX; offsetX=0; } if (offsetZ<0) { sizeZ += offsetZ; offsetZ=0; } //Not Tested!
			int res = data.heightmapResolution;
			if (sizeX+offsetX > res) sizeX = res-offsetX; if (sizeZ+offsetZ > res) sizeZ = res-offsetZ;
			return data.GetHeights(offsetX, offsetZ, sizeX, sizeZ);
		}

		public static float[,,] SafeGetAlphamaps (this TerrainData data, int offsetX, int offsetZ, int sizeX, int sizeZ)
		{
			if (offsetX<0) { sizeX += offsetX; offsetX=0; } if (offsetZ<0) { sizeZ += offsetZ; offsetZ=0; } //Not Tested!
			int res = data.alphamapResolution;
			if (sizeX+offsetX > res) sizeX = res-offsetX; if (sizeZ+offsetZ > res) sizeZ = res-offsetZ;
			return data.GetAlphamaps(offsetX, offsetZ, sizeX, sizeZ);
		}

		public static float GetInterpolated (this float[,] array, float x, float z)
		/// Gets value in-between pixels using linear interpolation. X and Z are swapped.
		{
			//neig coords
			int px = (int)x; if (x<0) px--; //because (int)-2.5 gives -2, should be -3 
			int nx = px+1;

			int pz = (int)z; if (z<0) pz--; 
			int nz = pz+1;

			//reading values
			float val_pxpz = array[px, pz];  //x and z swapped
			float val_nxpz = array[px, nz];
			float val_pxnz = array[nx, pz];
			float val_nxnz = array[nx, nz];

			float percentX = x-px;
			float percentZ = z-pz;

			float val_fz = val_pxpz*(1-percentX) + val_nxpz*percentX;
			float val_cz = val_pxnz*(1-percentX) + val_nxnz*percentX;
			float val = val_fz*(1-percentZ) + val_cz*percentZ;

			return val;
		}





		public static bool Equal (Vector3 v1, Vector3 v2)
		{
			return Mathf.Approximately(v1.x, v2.x) && 
					Mathf.Approximately(v1.y, v2.y) && 
					Mathf.Approximately(v1.z, v2.z);
		}
		
		public static bool Equal (Ray r1, Ray r2)
		{
			return Equal(r1.origin, r2.origin) && Equal(r1.direction, r2.direction);
		}


		public static Vector3[] InverseTransformPoint (this Transform tfm, Vector3[] points)
		{
			for (int c=0; c<points.Length; c++) points[c] = tfm.InverseTransformPoint(points[c]);
			return points;
		}

		public static Vector3 GetCenter (this Vector3[] poses)
		{
			if (poses.Length == 0) return new Vector3();
			if (poses.Length == 1) return poses[0];

			float x=0; float y=0; float z=0;
			for (int i=0; i<poses.Length; i++)
			{
				x+=poses[i].x;
				y+=poses[i].y;
				z+=poses[i].z;
			}
			return new Vector3(x/poses.Length, y/poses.Length, z/poses.Length);
		}

		public static bool Approximately (Rect r1, Rect r2)
		{
			return  Mathf.Approximately(r1.x, r2.x) &&
					Mathf.Approximately(r1.y, r2.y) &&
					Mathf.Approximately(r1.width, r2.width) &&
					Mathf.Approximately(r1.height, r2.height);
		}

		public static IEnumerable<Vector3> CircleAround (this Vector3 center, float radius, int numPoints, bool endWhereStart=false)
		{
			float radianStep = 2*Mathf.PI / numPoints;
			if (endWhereStart) numPoints++;
			for (int i=0; i<numPoints; i++)
			{
				float angle = i*radianStep;
				Vector3 dir = new Vector3( Mathf.Sin(angle), 0, Mathf.Cos(angle) );
				yield return center + dir*radius;
			}
		}
	}
}