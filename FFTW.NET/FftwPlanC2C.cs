﻿#region Copyright and License
/*
This file is part of FFTW.NET, a wrapper around the FFTW library
for the .NET framework.
Copyright (C) 2017 Tobias Meyer

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/
#endregion

using System;
using System.Numerics;

namespace FFTW.NET
{
	public sealed class FftwPlanC2C : FftwPlan<Complex, Complex>
	{
		public IPinnedArray<Complex> Input => Buffer1;
		public IPinnedArray<Complex> Output => Buffer2;

		FftwPlanC2C(IPinnedArray<Complex> input, IPinnedArray<Complex> output, int rank, int[] n, bool verifyRankAndSize, DftDirection direction, PlannerFlags plannerFlags, int nThreads)
			: base(input, output, rank, n, verifyRankAndSize, direction, plannerFlags, nThreads) { }

		protected override IntPtr GetPlan(int rank, int[] n, IntPtr input, IntPtr output, DftDirection direction, PlannerFlags plannerFlags)
		{
			return FftwInterop.fftw_plan_dft(rank, n, input, output, direction, plannerFlags);
		}

		protected override void VerifyRankAndSize(IPinnedArray<Complex> input, IPinnedArray<Complex> output)
		{
			if (input.Rank != output.Rank)
				throw new ArgumentException($"{nameof(input)} and {nameof(output)} must have the same rank and size.");
			for (int i = 0; i < input.Rank; i++)
			{
				if (input.GetLength(i) != output.GetLength(i))
					throw new ArgumentException($"{nameof(input)} and {nameof(output)} must have the same rank and size.");
			}
		}

		protected override void VerifyMinSize(IPinnedArray<Complex> input, IPinnedArray<Complex> output, int[] n)
		{
			int size = Utils.GetTotalSize(n);

			if (input.Length < size)
				throw new ArgumentException($"{nameof(input)} is too small.");

			if (output.Length < size)
				throw new ArgumentException($"{nameof(output)} is too small.");
		}

		/// <summary>
		/// Initializes a new plan using the provided input and output buffers.
		/// These buffers may be overwritten during initialization.
		/// </summary>
		public static FftwPlanC2C Create(IPinnedArray<Complex> input, IPinnedArray<Complex> output, DftDirection direction, PlannerFlags plannerFlags = PlannerFlags.Default, int nThreads = 1)
		{
			FftwPlanC2C plan = new FftwPlanC2C(input, output, input.Rank, input.GetSize(), true, direction, plannerFlags, nThreads);
			if (plan.IsZero)
				return null;
			return plan;
		}

		internal static FftwPlanC2C Create(IPinnedArray<Complex> input, IPinnedArray<Complex> output, int rank, int[] n, DftDirection direction, PlannerFlags plannerFlags, int nThreads)
		{
			FftwPlanC2C plan = new FftwPlanC2C(input, output, rank, n, false, direction, plannerFlags, nThreads);
			if (plan.IsZero)
				return null;
			return plan;
		}
	}
}