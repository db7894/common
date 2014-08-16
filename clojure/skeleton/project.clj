(defproject skeleton "0.1.0"
  :description "A collection of clojure commons"
  :url "http://example.com/FIXME"
  :license {
    :name "Eclipse Public License"
    :url "http://www.eclipse.org/legal/epl-v10.html"}
  :dependencies [
    [org.clojure/clojure "1.6.0"]
    [org.clojure/core.logic "0.8.8"]
  ]
  :main ^:skip-aot skeleton.core
  :target-path "target/%s"
  :profiles {:uberjar {:aot :all}})
