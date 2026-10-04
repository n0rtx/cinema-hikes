import { Typography, Button } from "antd";
import { useNavigate } from "react-router-dom";
import { ToolOutlined, ArrowLeftOutlined } from "@ant-design/icons";

const { Title, Text } = Typography;

export const UnderConstructionPage = () => {
  const navigate = useNavigate();

  return (
    <div
      style={{
        minHeight: "70vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        textAlign: "center",
        padding: "40px 20px",
      }}
    >
      <div 
        style={{ 
          background: "rgba(229, 9, 20, 0.1)", 
          borderRadius: "50%", 
          width: "120px", 
          height: "120px", 
          display: "flex", 
          alignItems: "center", 
          justifyContent: "center",
          marginBottom: "24px"
        }}
      >
        <ToolOutlined style={{ fontSize: "60px", color: "#E50914" }} />
      </div>

      <Title level={2} style={{ color: "#fff", marginBottom: "16px", fontWeight: 800 }}>
        Captain, we're still building this deck!
      </Title>
      
      <Text 
        style={{ 
          color: "#aaa", 
          fontSize: "16px", 
          maxWidth: "500px", 
          display: "block", 
          marginBottom: "40px",
          lineHeight: "1.6"
        }}
      >
        The page you are looking for is currently under construction. Our crew is working hard to hoist the sails and get it ready for you. Check back soon!
      </Text>

      <Button
        type="primary"
        size="large"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate("/")}
        style={{
          backgroundColor: "#E50914",
          borderColor: "#E50914",
          fontWeight: 600,
          height: "48px",
          padding: "0 32px",
          fontSize: "16px",
        }}
      >
        Back to Safe Waters
      </Button>
    </div>
  );
};