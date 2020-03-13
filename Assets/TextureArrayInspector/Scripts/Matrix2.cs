using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TextureArrayInspector 
{

	public class Matrix2<T> : ICloneable
	{
		public CoordRect rect; //never assign it's size manually, use ChangeRect
		public int count;
		public T[] array;
		
		public int pos;
		 //rect.size.x*rect.size.z, not a property for faster access

		#region Creation

			public Matrix2 () {}

			public Matrix2 (int x, int z, T[] array=null)
			{
				rect = new CoordRect(0,0,x,z);
				count = x*z;
				if (array != null && array.Length<count) Debug.LogError("Array length: " + array.Length + " is lower then matrix capacity: " + count);
				if (array != null && array.Length>=count) this.array = array;
				else this.array = new T[count];
			}
		
			public Matrix2 (CoordRect rect, T[] array=null)
			{
				this.rect = rect;
				count = rect.size.x*rect.size.z;
				if (array != null && array.Length<count) Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
				if (array != null && array.Length>=count) this.array = array;
				else this.array = new T[count];
			}

			public Matrix2 (Coord offset, Coord size, T[] array=null)
			{
				rect = new CoordRect(offset, size);
				count = rect.size.x*rect.size.z;
				if (array != null && array.Length<count) Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
				if (array != null && array.Length>=count) this.array = array;
				else this.array = new T[count];
			}

		#endregion
		
		public T this[int x, int z] 
		{
			get { return array[(z-rect.offset.z)*rect.size.x + x - rect.offset.x]; } //rect fn duplicated to increase performance
			set { array[(z-rect.offset.z)*rect.size.x + x - rect.offset.x] = value; }
		}

		public T this[float x, float z] 
		///Floors coordinates and gets value. To get interpolated value use GetInterpolated
		{
			get{ 
				int ix = (int)(x); if (x<1) ix--;
				int iz = (int)(z); if (z<1) iz--;
				return array[(iz-rect.offset.z)*rect.size.x + ix - rect.offset.x]; 
			}
			set{ 
				int ix = (int)(x); if (x<1) ix--;
				int iz = (int)(z); if (z<1) iz--;
				array[(iz-rect.offset.z)*rect.size.x + ix - rect.offset.x] = value; 
			}
		}

		public T this[Coord c] 
		{
			get { return array[(c.z-rect.offset.z)*rect.size.x + c.x - rect.offset.x]; }
			set { array[(c.z-rect.offset.z)*rect.size.x + c.x - rect.offset.x] = value; }
		}

		public T CheckGet (int x, int z) 
		{ 
			if (x>=rect.offset.x && x<rect.offset.x+rect.size.x && z>=rect.offset.z && z<rect.offset.z+rect.size.z)
				return array[(z-rect.offset.z)*rect.size.x + x - rect.offset.x]; 
			else return default(T);
		} 

		/*public T this[Vector3 pos]
		{
			get { return array[((int)pos.z-rect.offset.z)*rect.size.x + (int)pos.x - rect.offset.x]; }
			set { array[((int)pos.z-rect.offset.z)*rect.size.x + (int)pos.x - rect.offset.x] = value; }
		}*/

		public T this[Vector2 pos]
		{
			get{ 
				int posX = (int)(pos.x + 0.5f); if (pos.x < 0) posX--;
				int posZ = (int)(pos.y + 0.5f); if (pos.y < 0) posZ--;
				return array[(posZ-rect.offset.z)*rect.size.x + posX - rect.offset.x]; 
			}
			set{
				int posX = (int)(pos.x + 0.5f); if (pos.x < 0) posX--;
				int posZ = (int)(pos.y + 0.5f); if (pos.y < 0) posZ--;
				array[(posZ-rect.offset.z)*rect.size.x + posX - rect.offset.x] = value; 
			}
		}

		public void Clear () { for (int i=0; i<array.Length; i++) array[i] = default(T); }

		public void ChangeRect (CoordRect newRect, bool forceNewArray=false) //will re-create array only if capacity changed
		{
			rect = newRect;
			count = newRect.size.x*newRect.size.z;

			if (array.Length<count || forceNewArray) array = new T[count];
		}

		public virtual object Clone () { return Clone(null); } //separate fn for IClonable
		public Matrix2<T> Clone (Matrix2<T> result)
		{
			if (result==null) result = new Matrix2<T>(rect);
			
			//copy params
			result.rect = rect;
			result.pos = pos;
			result.count = count;
			
			//copy array
			//result.array = (float[])array.Clone(); //no need to create it any time
			if (result.array.Length != array.Length) result.array = new T[array.Length];
			for (int i=0; i<array.Length; i++)
				result.array[i] = array[i];

			return result;
		}

		public void Fill (T v) { for (int i=0; i<count; i++) array[i] = v; }

		public void Fill (Matrix2<T> m, bool removeBorders=false)
		{
			CoordRect intersection = CoordRect.Intersect(rect, m.rect);
			Coord min = intersection.Min; Coord max = intersection.Max;
			for (int x=min.x; x<max.x; x++)
				for (int z=min.z; z<max.z; z++)
					this[x,z] = m[x,z];
			if (removeBorders) RemoveBorders(intersection);
		}

		#region Quick Pos

			public void SetPos(int x, int z) { pos = (z-rect.offset.z)*rect.size.x + x - rect.offset.x; }
			public void SetPos(int x, int z, int s) { pos = (z-rect.offset.z)*rect.size.x + x - rect.offset.x  +  s*rect.size.x*rect.size.z; }

			public void MoveX() { pos++; }
			public void MoveZ() { pos += rect.size.x; }
			public void MovePrevX() { pos--; }
			public void MovePrevZ() { pos -= rect.size.x; }

			//public float current { get { return array[pos]; } set { array[pos] = value; } }
			/*public T nextX { get { return array[pos+1]; } set { array[pos+1] = value; } }
			public T prevX { get { return array[pos-1]; } set { array[pos-1] = value; } }
			public T nextZ { get { return array[pos+rect.size.x]; } set { array[pos+rect.size.x] = value; } }
			public T prevZ { get { return array[pos-rect.size.x]; } set { array[pos-rect.size.x] = value; } }
			public T nextXnextZ { get { return array[pos+rect.size.x+1]; } set { array[pos+rect.size.x+1] = value; } }
			public T prevXnextZ { get { return array[pos+rect.size.x-1]; } set { array[pos+rect.size.x-1] = value; } }
			public T nextXprevZ { get { return array[pos-rect.size.x+1]; } set { array[pos-rect.size.x+1] = value; } }
			public T prevXprevZ { get { return array[pos-rect.size.x-1]; } set { array[pos-rect.size.x-1] = value; } }*/

		#endregion

		#region OrderedFromCenter

			/*public Coord[] GetOrderedFromCenterCoords ()
			{
				Coord[] sortedByDistance = new Coord[array.Length];
				int i=0;
				Coord min = rect.Min; Coord max = rect.Max;
				for (int x=min.x; x<max.x; x++)
					for (int z=min.z; z<max.z; z++)
						{ sortedByDistance[i] = new Coord(x,z); i++; }

				float[] distances = new float[array.Length];
				for (int z=0; z<rect.size.z; z++)
					for (int x=0; x<rect.size.x; x++)
						distances[z*rect.size.x + x] = (x-rect.size.x/2)*(x-rect.size.x/2) + (z-rect.size.z/2)*(z-rect.size.z/2); //Mathf.Max( Mathf.Abs(x-chunks.rect.size.x/2), Mathf.Abs(z-chunks.rect.size.z/2) );

				Extensions.ArrayQSort(sortedByDistance, distances);
				return sortedByDistance;
			}

			public IEnumerable<Coord> OrderedFromCenterCoord ()
			{
				Coord[] sortedByDistance = GetOrderedFromCenterCoords();
				for (int i=0; i<sortedByDistance.Length; i++)
					yield return sortedByDistance[i];
			}

			public IEnumerable<T> OrderedFromCenter ()
			{
				Coord[] sortedByDistance = GetOrderedFromCenterCoords();
				for (int i=0; i<sortedByDistance.Length; i++)
					yield return this[sortedByDistance[i]];
			}*/

		#endregion

		#region Borders

			public void RemoveBorders ()
			{
				Coord min = rect.Min; Coord last = rect.Max - 1;
			
				for (int x=min.x; x<=last.x; x++)
					{ SetPos(x,min.z); array[pos] = array[pos+rect.size.x]; }

				for (int x=min.x; x<=last.x; x++)
					{ SetPos(x,last.z); array[pos] = array[pos-rect.size.x]; }

				for (int z=min.z; z<=last.z; z++)
					{ SetPos(min.x,z); array[pos] = array[pos+1]; }

				for (int z=min.z; z<=last.z; z++)
					{ SetPos(last.x,z); array[pos] = array[pos-1]; }
			}

			public void RemoveBorders (int borderMinX, int borderMinZ, int borderMaxX, int borderMaxZ)
			{
				Coord min = rect.Min; Coord max = rect.Max;
			
				if (borderMinZ != 0)
				for (int x=min.x; x<max.x; x++)
				{
					T val = this[x, min.z+borderMinZ];
					for (int z=min.z; z<min.z+borderMinZ; z++) this[x,z] = val;
				}

				if (borderMaxZ != 0)
				for (int x=min.x; x<max.x; x++)
				{
					T val = this[x, max.z-borderMaxZ];
					for (int z=max.z-borderMaxZ; z<max.z; z++) this[x,z] = val;
				}

				if (borderMinX != 0)
				for (int z=min.z; z<max.z; z++)
				{
					T val = this[min.x+borderMinX, z];
					for (int x=min.x; x<min.x+borderMinX; x++) this[x,z] = val;
				}
				
				if (borderMaxX != 0)
				for (int z=min.z; z<max.z; z++)
				{
					T val = this[max.x-borderMaxX, z];
					for (int x=max.x-borderMaxX; x<max.x; x++) this[x,z] = val;
				}
			}

			public void RemoveBorders (CoordRect centerRect)
			{ 
				RemoveBorders(
					Mathf.Max(0,centerRect.offset.x-rect.offset.x), 
					Mathf.Max(0,centerRect.offset.z-rect.offset.z), 
					Mathf.Max(0,rect.Max.x-centerRect.Max.x+1), 
					Mathf.Max(0,rect.Max.z-centerRect.Max.z+1) ); 
			}

		#endregion
	}

}


