package org.school.core

import org.scalatest.FlatSpec
import org.scalatest.matchers.ShouldMatchers

class MainSpec extends FlatSpec with ShouldMatchers {

	behavior of "The main project"

	it should "do something correctly" in {
        true should be (true)
	}
}

