import { useParams, useNavigate } from "react-router-dom";
import { Typography, Row, Col, Card, Avatar, Tag, Button } from "antd";
import {
  ArrowLeftOutlined,
  GithubOutlined,
  StarFilled,
} from "@ant-design/icons";
import type{ DeveloperProfileDto } from "../../shared/types/DTO/DeveloperProfileDTO";
const { Title, Paragraph, Text } = Typography;
const DEVELOPER_DATA: Record<string, DeveloperProfileDto> = {
  b1mq: {
    username: "b1mq",
    role: "Backend & Frontend",
    age: 17,
    city: "Stuttgart,Germany",
    hobby: "Gym,Coding, UI/UX design",
    avatarURL: "https://github.com/b1mq.png",
    githubURL: "https://github.com/b1mq",
    gifURL:
      "https://camo.githubusercontent.com/ab4e1e7c0b9c43c44b78ba6714ec8bb49517c9db4b7e1a74d24ce8744acc31d9/68747470733a2f2f6d65646961302e67697068792e636f6d2f6d656469612f76312e59326c6b505463354d4749334e6a45784e4756354e6e70325a6d46784f57706c636e4e724e47566b5a5852794e6a6c6d4f474a6a4e4849784f4468326457357a646e42695a535a6c634431324d563970626e526c636d35686246396e61575a66596e6c666157516d593351395a772f4138556f3469643657436b38724a6b4a6b352f67697068792e676966",
    skills: [
      "C#",
      ".NET",
      "ASP.NET Core",
      "Python",
      "HTML & CSS",
      "Design Patterns",
      "TypeScript",
    ],
    favoriteMovies: [
      "Fast & Furios",
      "Avatar",
      "The Transporter",
      "Fight Club",
      "Mechanic",
    ],
  },
  n0rtx: {
    username: "n0rtx",
    role: "Backend & Frontend Developer",
    age: 16,
    city: "Odessa,Ukraine",
    hobby: "Software architecture, backend development, and tech automation",
    gifURL:"https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExcDA1amNkaXZwMnRkNXVoMmFyMHIwdHJ4dG5yNm96bmNxcGhqY2VxYiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/Ig6ziBl0Ygs7sOtYs8/giphy.gif",
    avatarURL: "https://github.com/n0rtx.png",
    githubURL: "https://github.com/n0rtx",
    skills: ["C#", ".NET Core", "SQL Server", "TypeScript", "React", "Git"],
    favoriteMovies: ["Inglourious Basterds", "Fight Club", "Gladiator"],
  },
};
export const DeveloperPage = () => {
  const { username } = useParams<{ username: string }>();
  const navigate = useNavigate();
  const dev = username ? DEVELOPER_DATA[username.toLowerCase()] : null;
  if (!dev) {
    return (
      <div style={{ textAlign: "center", marginTop: "100px" }}>
        <Title level={2} style={{ color: "#fff" }}>
          Developer not found
        </Title>
        <Button
          type="primary"
          onClick={() => navigate("/about")}
          style={{ backgroundColor: "#E50914", borderColor: "#E50914" }}
        >
          Back to About
        </Button>
      </div>
    );
  }
  return (
    <div style={{ maxWidth: "900px", margin: "0 auto", paddingTop: "24px" }}>
      <Button
        type="link"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate("/about")}
        style={{ marginBottom: "24px", paddingLeft: 0, color: "#999" }}
      >
        Back to About Us
      </Button>
      <Card
        style={{
          backgroundColor: "#1f1f1f",
          border: "none",
          borderRadius: "16px",
          padding: "24px",
        }}
      >
        <Row gutter={[32, 32]} align={"middle"}>
          <Col xs={24} md={8} style={{ textAlign: "center" }}>
            <Avatar
              size={140}
              src={dev.avatarURL}
              style={{ border: "3px solid #E50914", marginBottom: "16px" }}
            />
            <Title level={3} style={{ margin: "0 0 4px 0", color: "#fff" }}>
              {dev.username}
            </Title>
            <Text
              type="secondary"
              style={{ display: "block", marginBottom: "4px" }}
            >
              {dev.role}
            </Text>
            <Text
              type="secondary"
              style={{ display: "block", marginBottom: "16px" }}
            >
              {dev.city} & {dev.age} years old
            </Text>
            <Button
              type="primary"
              icon={<GithubOutlined />}
              href={dev.githubURL}
              target="_blank"
              style={{
                backgroundColor: "#333",
                borderColor: "#444",
                width: "100%",
              }}
            >
              GitHub Profile
            </Button>
          </Col>
          <Col xs={24} md={16}>
            <Title level={4} style={{ color: '#fff', borderBottom: '1px solid #333', paddingBottom: '8px' }}>
              Hobby & Interests
            </Title>
            <Paragraph style={{ color: '#ccc', fontSize: '16px', lineHeight: '1.6', marginTop: '12px' }}>
              {dev.hobby}
            </Paragraph>

            {dev.gifURL && (
              <div style={{ margin: '16px 0' }}>
                <img src={dev.gifURL} alt="animation" style={{ maxWidth: '100%', borderRadius: '8px' }} />
              </div>
            )}

            <Title level={4} style={{ color: '#fff', borderBottom: '1px solid #333', paddingBottom: '8px', marginTop: '24px' }}>
              Skills & Tech Stack
            </Title>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginTop: '12px' }}>
              {dev.skills.map(skill => (
                <Tag key={skill} color="red" style={{ fontSize: '14px', padding: '4px 10px', borderRadius: '4px' }}>
                  {skill}
                </Tag>
              ))}
            </div>

            <Title level={4} style={{ color: '#fff', borderBottom: '1px solid #333', paddingBottom: '8px', marginTop: '24px' }}>
              Favorite Movies
            </Title>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '8px', marginTop: '12px' }}>
              {dev.favoriteMovies.map(movie => (
                <div key={movie} style={{ display: 'flex', alignItems: 'center', gap: '8px', color: '#ddd' }}>
                  <StarFilled style={{ color: '#faad14' }} />
                  <span>{movie}</span>
                </div>
              ))}
            </div>
          </Col>
        </Row>
      </Card>
    </div>
  );
};
