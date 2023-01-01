window.BENCHMARK_DATA = {
  "lastUpdate": 1672583977499,
  "repoUrl": "https://github.com/kengwang/Depository",
  "entries": {
    "Benchmark.Net Benchmark": [
      {
        "commit": {
          "author": {
            "email": "atkengwang@qq.com",
            "name": "kengwang",
            "username": "kengwang"
          },
          "committer": {
            "email": "atkengwang@qq.com",
            "name": "kengwang",
            "username": "kengwang"
          },
          "distinct": false,
          "id": "401a804d5054aa466651436e003f25e2fd785e33",
          "message": "[fix]<ci/benchmark>: Fix failure",
          "timestamp": "2023-01-01T22:17:26+08:00",
          "tree_id": "f7263a921b0176e7abe4429c7e14e8adfc9533c5",
          "url": "https://github.com/kengwang/Depository/commit/401a804d5054aa466651436e003f25e2fd785e33"
        },
        "date": 1672583976999,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Depository.Benchmarks.Benchmarks.ResolveSingleToSingle",
            "value": 3303.2750475565595,
            "unit": "ns",
            "range": "± 42.661087792759744"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.ResolveMultipleToSingle",
            "value": 3680.2252289908274,
            "unit": "ns",
            "range": "± 50.28115307268083"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.ResolveMultipleToMultiple_UsingIEnumerable",
            "value": 5370.665058429425,
            "unit": "ns",
            "range": "± 49.07642803074226"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.ResolveMultipleToMultiple_UsingMultipleResolve",
            "value": 4696.7877103632145,
            "unit": "ns",
            "range": "± 114.7563671464063"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.Depository_MultiToMultiBenchmark_IEnumerable",
            "value": 5431.48841047287,
            "unit": "ns",
            "range": "± 99.75625666586757"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.Depository_MultiToMultiBenchmark_ResolveMultiple",
            "value": 4544.218385219574,
            "unit": "ns",
            "range": "± 86.53235381462532"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.MicrosoftExtensionDependencyInjection_MultiToMultiBenchmark",
            "value": 5081.204828872681,
            "unit": "ns",
            "range": "± 378.2113324359217"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.AutoFac_MultiToMultiBenchmark",
            "value": 128531.98090820313,
            "unit": "ns",
            "range": "± 3743.5347179541914"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.Depository_MultiToSingleBenchmark",
            "value": 3741.6139640808105,
            "unit": "ns",
            "range": "± 31.46200174306317"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.MicrosoftExtensionDependencyInjection_MultiToSingleBenchmark",
            "value": 4079.819647979736,
            "unit": "ns",
            "range": "± 636.6452993299071"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.AutoFac_MultiToSingleBenchmark",
            "value": 38096.52329101563,
            "unit": "ns",
            "range": "± 367.24293489377413"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.Depository_SingleToDefaultImplementBenchmark",
            "value": 2652.4566696166994,
            "unit": "ns",
            "range": "± 47.80883598147131"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.MicrosoftExtensionDependencyInjection_SingleToDefaultImplementBenchmark",
            "value": 3382.2409964370727,
            "unit": "ns",
            "range": "± 588.0519799080081"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.AutoFac_SingleToDefaultImplementBenchmark",
            "value": 28843.1280670166,
            "unit": "ns",
            "range": "± 88.52537867265298"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.Depository",
            "value": 3419.501790727888,
            "unit": "ns",
            "range": "± 48.17498542937093"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.MicrosoftExtensionDependencyInjection",
            "value": 3817.0704342269896,
            "unit": "ns",
            "range": "± 693.0453602534305"
          },
          {
            "name": "Depository.Benchmarks.Benchmarks.AutoFac",
            "value": 30389.17167154948,
            "unit": "ns",
            "range": "± 368.61615626890136"
          }
        ]
      }
    ]
  }
}