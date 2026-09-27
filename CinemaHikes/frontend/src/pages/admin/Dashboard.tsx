import { Typography, Row, Col, Card, Statistic } from "antd";

const { Title } = Typography;

export const DashboardPage = () => {
  return (
    <div>
      <Title level={2} style={{ marginBottom: "24px" }}>
        Platform Overview
      </Title>
      <Row gutter={[16,16]}>
        <Col xs={24} sm={12} md={6}>
            <Card style={{boxShadow:"0 2px 8px rgba(0,0,0,0.08)"}}>
                <Statistic title="Total Movies">
    
                </Statistic>
            </Card>
        </Col>
      </Row>
    </div>
  );
};
