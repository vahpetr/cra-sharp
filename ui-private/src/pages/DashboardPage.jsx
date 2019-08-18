import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { Layout, Menu, Breadcrumb, Icon } from 'antd';
import Application from '../modules/Application';
import styled from 'styled-components/macro';

export class DashboardPage extends React.Component {
  static propTypes = {
    history: PropTypes.shape({
      push: PropTypes.func.isRequired
    }).isRequired,
    user: PropTypes.shape({
      role: PropTypes.string.isRequired,
      email: PropTypes.string.isRequired
    })
  };

  static defaultProps = {
    user: null
  };

  state = {
    collapsed: false,
    data: null
  };

  componentDidMount() {
    this.loadData();
  }

  async loadData() {
    const data = await Application.dataProvider.getData();
    this.setState({ data });
  }

  onCollapse = collapsed => {
    this.setState({ collapsed });
  };

  onLogout = () => this.props.history.push('/logout');

  render() {
    const { user } = this.props;
    const { data } = this.state;

    return (
      <Layout>
        <Layout.Header>
          <Logo />
          <StyledHeaderMenu>
            <Logout key="1" onClick={this.onLogout}>
              Выход
            </Logout>
          </StyledHeaderMenu>
        </Layout.Header>
        <StyledLayout>
          <Layout.Sider
            collapsible
            collapsed={this.state.collapsed}
            onCollapse={this.onCollapse}
          >
            <StyledNavMenu defaultSelectedKeys={['1']}>
              <Menu.Item key="1">
                <Icon type="dashboard" />
                <span>Дашборд</span>
              </Menu.Item>
              <Menu.Item key="2">
                <Icon type="pie-chart" />
                <span>Графики</span>
              </Menu.Item>
              <Menu.SubMenu
                key="sub1"
                title={
                  <span>
                    <Icon type="user" />
                    <span>Пользователи</span>
                  </span>
                }
              >
                <Menu.Item key="3">Пользователь 1</Menu.Item>
                <Menu.Item key="4">Пользователь 2</Menu.Item>
                <Menu.Item key="5">Пользователь 3</Menu.Item>
              </Menu.SubMenu>
              <Menu.SubMenu
                key="sub2"
                title={
                  <span>
                    <Icon type="team" />
                    <span>Команды</span>
                  </span>
                }
              >
                <Menu.Item key="6">Команда 1</Menu.Item>
                <Menu.Item key="8">Команда 2</Menu.Item>
              </Menu.SubMenu>
              <Menu.Item key="9">
                <Icon type="file" />
                <span>Файлы</span>
              </Menu.Item>
            </StyledNavMenu>
          </Layout.Sider>
          <Layout>
            <StyledContent>
              {user ? (
                <StyledBreadcrumb>
                  <Breadcrumb.Item>{user.role}</Breadcrumb.Item>
                  <Breadcrumb.Item>{user.email}</Breadcrumb.Item>
                </StyledBreadcrumb>
              ) : (
                <StyledBreadcrumb>
                  <Breadcrumb.Item>Загрузка...</Breadcrumb.Item>
                </StyledBreadcrumb>
              )}
              {data ? (
                <DataContent>{data}</DataContent>
              ) : (
                <DataContent>Загрузка...</DataContent>
              )}
            </StyledContent>
            <StyledFooter>Футер © 2019</StyledFooter>
          </Layout>
        </StyledLayout>
      </Layout>
    );
  }
}

const mapStateToProps = state => ({
  user: state.user
});

export default connect(mapStateToProps)(DashboardPage);

const DataContent = styled.div`
  padding: 24px;
  background: #fff;
  min-height: 360px;
`;

const Logo = styled.div`
  width: 120px;
  height: 31px;
  background: rgba(255, 255, 255, 0.2);
  margin: 16px 28px 16px 0;
  float: left;
`;

const StyledHeaderMenu = styled(Menu).attrs({
  theme: 'dark',
  mode: 'horizontal'
})`
  line-height: 64px;
`;

const StyledNavMenu = styled(Menu).attrs({
  theme: 'dark',
  mode: 'inline'
})`
  /* nop */
`;

const Logout = styled(Menu.Item)`
  float: right;
`;

const StyledContent = styled(Layout.Content)`
  margin: 0 16px;
`;

const StyledBreadcrumb = styled(Breadcrumb)`
  margin: 16px 0;
`;

const StyledFooter = styled(Layout.Footer)`
  text-align: center;
`;

const StyledLayout = styled(Layout)`
  min-height: 100vh;
`;
