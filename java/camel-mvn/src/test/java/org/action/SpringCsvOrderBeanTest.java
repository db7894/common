package org.action;

public class SpringCsvOrderBeanTest extends CamelSpringTestSupport {

    @Override
    protected AbstractXmlApplicationContext createApplicationContext() {
        return new ClassPathXmlApplicationContext("org/action/SpringCsvOrderBeanTest.xml");
    }

    @Test
    public void testOrderToCsvBean() throws Exception {
        MockEndpoint mock = getMockEndpoint("mock:results");
    }
}
