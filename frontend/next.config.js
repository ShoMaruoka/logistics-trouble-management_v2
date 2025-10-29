/** @type {import('next').NextConfig} */
const nextConfig = {
  // サブパス対応設定（開発環境では無効化）
  // basePath: '/ltm', // /ltm/パスでアクセスするための設定
  
  // 本番環境での最適化設定
  output: process.env.NODE_ENV === 'production' && process.env.EXPORT_MODE === 'true' ? 'export' : 'standalone', // 静的エクスポート用
  compress: true, // gzip圧縮
  poweredByHeader: false, // X-Powered-Byヘッダーを削除
  
  images: {
    domains: ['localhost'],
    formats: ['image/webp', 'image/avif'],
  },
  
  // 本番環境での環境変数設定
  env: {
    CUSTOM_KEY: process.env.CUSTOM_KEY,
  },
  
  async headers() {
    return [
      {
        source: '/(.*)',
        headers: [
          {
            key: 'X-Frame-Options',
            value: 'DENY',
          },
          {
            key: 'X-Content-Type-Options',
            value: 'nosniff',
          },
          {
            key: 'Referrer-Policy',
            value: 'origin-when-cross-origin',
          },
          {
            key: 'X-XSS-Protection',
            value: '1; mode=block',
          },
          {
            key: 'Strict-Transport-Security',
            value: 'max-age=31536000; includeSubDomains',
          },
        ],
      },
    ]
  },
}

module.exports = nextConfig
