(defproject reasoned "0.1.0"
  :description "Walking through teh reasoned schemer"
  :url "https://github.com/clojure/core.logic"
  :license {
    :name "Eclipse Public License"
    :url "http://www.eclipse.org/legal/epl-v10.html"}
  :dependencies [
    [org.clojure/clojure "1.6.0"]
    [org.clojure/core.logic "0.8.8"]
  ]
  :main ^:skip-aot reasoned.core
  :target-path "target/%s"
  :profiles {:uberjar {:aot :all}})
